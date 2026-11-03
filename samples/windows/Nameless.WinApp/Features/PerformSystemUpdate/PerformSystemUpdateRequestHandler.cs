using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Nameless.Contracts;
using Nameless.IO;
using Nameless.Lucene;
using Nameless.Mediator.Requests;
using Nameless.ObjectModel;
using Nameless.WinApp.Data;
using Nameless.WinApp.Features.CheckSystemUpdate;
using Nameless.WinApp.Infrastructure;
using Nameless.WinApp.Infrastructure.GitHub;
using Nameless.Windows;
using Nameless.Windows.Events;
using Nameless.Windows.Localization;
using Nameless.Windows.Notification;
using Nameless.Windows.Notification.SnackBar;

namespace Nameless.WinApp.Features.PerformSystemUpdate;

/// <summary>
///     Default handler for <see cref="PerformSystemUpdateRequest"/>.
/// </summary>
public class PerformSystemUpdateRequestHandler : IRequestHandler<PerformSystemUpdateRequest, SystemUpdateOutcome> {
    // One top-level install-directory entry that was actually swapped into
    // place during this run. BackupPath is null when the destination did
    // not exist before the swap — rollback for that case is "delete what
    // was placed", not "move a backup sibling back".
    private readonly record struct EntrySwap(string OriginalPath, string? BackupPath, bool IsDirectory);

    private readonly IGitHubReleaseClient _gitHubReleaseClient;
    private readonly ISystemUpdateEnvironment _environment;
    private readonly IFileManager _fileManager;
    private readonly AppDbContext _dbContext;
    private readonly IIndexProvider _indexProvider;
    private readonly IEventPublisher _eventPublisher;
    private readonly ITranslator _translator;
    private readonly ILogger<PerformSystemUpdateRequestHandler> _logger;

    /// <summary>
    ///     Initializes a new instance of
    ///     <see cref="PerformSystemUpdateRequestHandler"/> class.
    /// </summary>
    public PerformSystemUpdateRequestHandler(
        IGitHubReleaseClient gitHubReleaseClient,
        ISystemUpdateEnvironment environment,
        IFileManager fileManager,
        AppDbContext dbContext,
        IIndexProvider indexProvider,
        IEventPublisher eventPublisher,
        ITranslator translator,
        ILogger<PerformSystemUpdateRequestHandler> logger) {
        _gitHubReleaseClient = Throws.When.Null(gitHubReleaseClient);
        _environment = Throws.When.Null(environment);
        _fileManager = Throws.When.Null(fileManager);
        _dbContext = Throws.When.Null(dbContext);
        _indexProvider = Throws.When.Null(indexProvider);
        _eventPublisher = Throws.When.Null(eventPublisher);
        _translator = Throws.When.Null(translator);
        _logger = Throws.When.Null(logger);
    }

    /// <inheritdoc />
    public async Task<SystemUpdateOutcome> HandleAsync(PerformSystemUpdateRequest request, CancellationToken cancellationToken) {
        Throws.When.Null(request);

        var release = request.Release;
        var timestamp = DateTimeOffset.UtcNow;

        // Resolved up front (cheap — no disk I/O) so the `finally` below can
        // always find and clean them up, even if a later step fails or is
        // cancelled.
        var stagingPath = SysPath.Combine(_environment.InstallDirectoryPath, string.Format(Constants.SystemUpdate.StagingDirectoryPattern, timestamp));
        var packagePath = SysPath.Combine(_fileManager.GetUpdateDirectory().Path, release.Package.Name);

        var swaps = new List<EntrySwap>();

        // Once true, nothing below can turn into a rollback: the swapped
        // files are the new source of truth. See the catch filters below
        // and the guarded tail that follows the commit.
        var committed = false;

        try {
            await PublishProgressAsync("Starting");

            await RunStepAsync("CheckingPrerequisites", () => {
                CheckPrerequisites(timestamp);

                return Task.CompletedTask;
            });

            string? expectedHash = null;

            await RunStepAsync("DownloadingChecksum", async () => {
                expectedHash = await DownloadAndParseChecksumAsync(release, cancellationToken);
            });

            await RunStepAsync("DownloadingPackage", () => DownloadPackageAsync(release, packagePath, cancellationToken));

            await RunStepAsync("VerifyingPackage", () => VerifyPackageAsync(release, packagePath, expectedHash!, cancellationToken));

            await RunStepAsync("ExtractingPackage", () => ExtractPackageAsync(packagePath, stagingPath, cancellationToken));

            await RunStepAsync("ValidatingPackage", () => {
                ValidatePackage(stagingPath, release);

                return Task.CompletedTask;
            });

            await RunStepAsync("ClosingDatabaseConnection", () => _dbContext.Database.CloseConnectionAsync());

            await RunStepAsync("ClosingLuceneIndex", () => {
                _indexProvider.CloseAll();

                return Task.CompletedTask;
            });

            // From here on, cancellation is no longer observed: a cancel
            // landing mid-swap would only trigger a rollback of work that
            // already succeeded (plan §3.4).
            await RunStepAsync("SwappingFiles", () => SwapFilesAsync(stagingPath, timestamp, swaps));

            await RunStepAsync("VerifyingInstallation", () => {
                VerifyInstallation(release);

                return Task.CompletedTask;
            });

            // Commit point: once set, a late failure must never undo the
            // swapped files or report RolledBack for an update that already
            // succeeded. Everything from here on runs in its own
            // best-effort guard and the catch blocks below are filtered out
            // by `when (!committed)`.
            committed = true;

            await CompleteCommittedUpdateAsync();

            return SystemUpdateOutcome.Completed;
        }
        catch (OperationCanceledException) when (!committed) {
            await RollbackSwapsAsync(swaps);

            await _eventPublisher.PublishFailureAsync<SnackBarNotificationEvent>(
                message: _translator[$"{Prefix}_RolledBack_Cancelled"]
            );

            return SystemUpdateOutcome.RolledBack;
        }
        catch (Exception ex) when (!committed) {
            await RollbackSwapsAsync(swaps);

            await _eventPublisher.PublishFailureAsync<SnackBarNotificationEvent>(
                message: string.Format(_translator[$"{Prefix}_RolledBack"], ex.Message)
            );

            return SystemUpdateOutcome.RolledBack;
        }
        finally {
            DeleteDirectoryBestEffort(stagingPath);
            DeleteFileBestEffort(packagePath);
        }
    }

    // Runs everything that happens after the commit point. A failure here
    // is cosmetic (the update already succeeded) and must never escape as
    // an exception or be mistaken for a reason to roll back (lesson from
    // #163).
    private async Task CompleteCommittedUpdateAsync() {
        try {
            await PublishProgressAsync("Success");
        }
        catch (Exception ex) {
            CommonLog.Error(_logger, $"Post-commit success notification failed: {ex.Message}", ex, tag: GetType().Tag);
        }
    }

    private async Task RunStepAsync(string stepName, Func<Task> action) {
        await PublishProgressAsync(stepName);

        try {
            await action();
        }
        catch (Exception ex) when (ex is not OperationCanceledException) {
            await PublishStepFailureAsync(stepName, ex);

            throw;
        }
    }

    private void CheckPrerequisites(DateTimeOffset timestamp) {
        if (!_environment.IsSelfUpdateEnabled) {
            throw new InvalidOperationException("Self-update is only available in release builds.");
        }

        var executableDirectoryPath = SysPath.GetDirectoryName(_environment.ExecutablePath) ?? string.Empty;
        var normalizedExecutableDirectoryPath = PathHelper.RemoveTrailingSlash(PathHelper.Normalize(executableDirectoryPath));
        var executableName = SysPath.GetFileName(_environment.ExecutablePath);

        if (!string.Equals(normalizedExecutableDirectoryPath, _environment.InstallDirectoryPath, StringComparison.OrdinalIgnoreCase) ||
            !string.Equals(executableName, Constants.SystemUpdate.ExecutableName, StringComparison.OrdinalIgnoreCase)) {
            throw new InvalidOperationException("The running executable is not where the self-update feature expects it to be.");
        }

        var probePath = SysPath.Combine(_environment.InstallDirectoryPath, $".update-probe-{timestamp:yyyyMMddHHmmss}");

        try {
            SysFile.WriteAllText(probePath, string.Empty);
            SysFile.Delete(probePath);
        }
        catch (UnauthorizedAccessException ex) {
            throw new InvalidOperationException(
                "The install folder is not writable (for example under Program Files). Move the app or reinstall it in a user-writable folder.", ex
            );
        }
    }

    private async Task<string> DownloadAndParseChecksumAsync(AvailableRelease release, CancellationToken cancellationToken) {
        var text = await _gitHubReleaseClient.DownloadTextAsync(release.Checksum.DownloadUrl, maxBytes: 1024, cancellationToken);

        return ParseChecksumSidecar(text, release.Package.Name);
    }

    // Release contract §2.2: one line, GNU sha256sum format, BOM allowed
    // and ignored, trailing newline optional. The hash token must be
    // exactly 64 hex characters; a present file-name token must match the
    // package asset name.
    private static string ParseChecksumSidecar(string text, string expectedFileName) {
        var trimmed = text.TrimStart('﻿').Trim();
        var tokens = trimmed.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);

        if (tokens.Length == 0) {
            throw new InvalidDataException("The checksum sidecar is empty.");
        }

        var hash = tokens[0];

        if (hash.Length != 64 || !IsHex(hash)) {
            throw new InvalidDataException($"The checksum sidecar's hash token is not 64 hex characters: '{hash}'.");
        }

        if (tokens.Length > 1 && !string.Equals(tokens[1], expectedFileName, StringComparison.Ordinal)) {
            throw new InvalidDataException(
                $"The checksum sidecar's file name token '{tokens[1]}' does not match the expected package name '{expectedFileName}'."
            );
        }

        return hash.ToLowerInvariant();
    }

    private static bool IsHex(string value) {
        foreach (var @char in value) {
            if (!Uri.IsHexDigit(@char)) { return false; }
        }

        return true;
    }

    private async Task DownloadPackageAsync(AvailableRelease release, string packagePath, CancellationToken cancellationToken) {
        var directoryPath = SysPath.GetDirectoryName(packagePath);

        if (directoryPath is not null) { SysDirectory.CreateDirectory(directoryPath); }

        await _gitHubReleaseClient.DownloadToFileAsync(release.Package.DownloadUrl, packagePath, cancellationToken);
    }

    private static async Task VerifyPackageAsync(AvailableRelease release, string packagePath, string expectedHash, CancellationToken cancellationToken) {
        var length = new FileInfo(packagePath).Length;

        if (length != release.Package.Size) {
            throw new InvalidDataException(
                $"Downloaded package size ({length} bytes) does not match the release's expected size ({release.Package.Size} bytes)."
            );
        }

        await using var stream = SysFile.OpenRead(packagePath);
        var hashBytes = await SHA256.HashDataAsync(stream, cancellationToken);
        var hash = Convert.ToHexStringLower(hashBytes);

        if (!string.Equals(hash, expectedHash, StringComparison.Ordinal)) {
            throw new InvalidDataException("The downloaded package's checksum does not match the release's checksum sidecar.");
        }
    }

    private static async Task ExtractPackageAsync(string packagePath, string stagingPath, CancellationToken cancellationToken) {
        await ZipFile.ExtractToDirectoryAsync(
            sourceArchiveFileName: packagePath,
            destinationDirectoryName: stagingPath,
            overwriteFiles: false,
            cancellationToken
        );
    }

    // Release contract §2.1: the executable must be at the root, and no
    // forbidden entry (runtime data folders, the updater's own working
    // names) may be present. The staged exe's stamped product version must
    // match the release's version — reusing InvalidDataException keeps this
    // consistent with the malformed-release path.
    private static void ValidatePackage(string stagingPath, AvailableRelease release) {
        var executablePath = SysPath.Combine(stagingPath, Constants.SystemUpdate.ExecutableName);

        if (!SysFile.Exists(executablePath)) {
            throw new InvalidDataException($"The package does not contain '{Constants.SystemUpdate.ExecutableName}' at its root.");
        }

        foreach (var entryPath in SysDirectory.EnumerateFileSystemEntries(stagingPath)) {
            var entryName = SysPath.GetFileName(entryPath);

            if (IsForbiddenRootEntry(entryName)) {
                throw new InvalidDataException($"The package contains a forbidden root entry: '{entryName}'.");
            }
        }

        var productVersion = FileVersionInfo.GetVersionInfo(executablePath).ProductVersion;

        if (!SemVersion.TryParse(productVersion, out var parsed) || parsed != release.Version) {
            throw new InvalidDataException(
                $"The staged executable's product version ('{productVersion}') does not match the release version ('{release.Version}')."
            );
        }
    }

    private static bool IsForbiddenRootEntry(string entryName) {
        foreach (var forbidden in Constants.SystemUpdate.ForbiddenRootEntries) {
            if (string.Equals(entryName, forbidden, StringComparison.OrdinalIgnoreCase)) { return true; }
        }

        return entryName.StartsWith(".update-", StringComparison.OrdinalIgnoreCase) ||
               entryName.Contains(".old-", StringComparison.OrdinalIgnoreCase);
    }

    // Swaps every top-level staging entry into the install directory, the
    // running executable last. Each existing destination is renamed to a
    // ".old-{ts}" sibling first; on a move failure that entry self-heals
    // (its own backup is moved back) before the caller's outer rollback
    // handles whatever else already succeeded.
    private async Task SwapFilesAsync(string stagingPath, DateTimeOffset timestamp, List<EntrySwap> swaps) {
        var entryNames = SysDirectory.EnumerateFileSystemEntries(stagingPath)
                                     .Select(SysPath.GetFileName)
                                     .OrderBy(name => string.Equals(name, Constants.SystemUpdate.ExecutableName, StringComparison.OrdinalIgnoreCase) ? 1 : 0)
                                     .ToArray();

        foreach (var name in entryNames) {
            if (name is null) { continue; }

            var stagedEntryPath = SysPath.Combine(stagingPath, name);
            var destinationPath = SysPath.Combine(_environment.InstallDirectoryPath, name);
            var isDirectory = SysDirectory.Exists(stagedEntryPath);

            await SwapEntryAsync(stagedEntryPath, destinationPath, isDirectory, timestamp, swaps);
        }
    }

    private async Task SwapEntryAsync(string stagedEntryPath, string destinationPath, bool isDirectory, DateTimeOffset timestamp, List<EntrySwap> swaps) {
        string? backupPath = null;

        var destinationExists = isDirectory ? SysDirectory.Exists(destinationPath) : SysFile.Exists(destinationPath);

        if (destinationExists) {
            backupPath = BuildOldSiblingPath(destinationPath, timestamp);

            MoveEntry(destinationPath, backupPath, isDirectory);
        }

        try {
            MoveEntry(stagedEntryPath, destinationPath, isDirectory);
        }
        catch {
            if (backupPath is not null) {
                await SelfHealAsync(() => MoveEntry(backupPath, destinationPath, isDirectory), destinationPath);
            }

            throw;
        }

        swaps.Add(new EntrySwap(destinationPath, backupPath, isDirectory));
    }

    // protected virtual so tests can force the move INTO the destination
    // (as opposed to the earlier rename-aside move) to fail deterministically,
    // exercising the self-heal branch in SwapEntryAsync's catch block.
    protected virtual void MoveEntry(string sourcePath, string destinationPath, bool isDirectory) {
        if (isDirectory) { SysDirectory.Move(sourcePath, destinationPath); }
        else { SysFile.Move(sourcePath, destinationPath); }
    }

    // protected virtual, same reason as MoveEntry: lets a test force one
    // rollback entry's delete to fail deterministically, so the per-entry
    // catch in RollbackSwapsAsync (and the loop continuing past it) can be
    // exercised without relying on file locks or timing.
    protected virtual void DeleteEntry(string path, bool isDirectory) {
        if (isDirectory) { SysDirectory.Delete(path, recursive: true); }
        else { SysFile.Delete(path); }
    }

    private async Task SelfHealAsync(Action moveBackupBack, string destinationPath) {
        try {
            moveBackupBack();
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException) {
            // Best-effort: the outer rollback (and the "RolledBack"/
            // "RolledBack_Cancelled" SnackBar message it publishes) still
            // runs against the original exception that triggered this
            // self-heal attempt. The ".old-*" sibling this leaves behind is
            // called out explicitly so it is not unaccounted for. The
            // publish itself is wrapped separately: a publisher that is
            // also down must not turn into an unhandled exception here.
            try {
                await _eventPublisher.PublishWarningAsync<SystemUpdateProgressNotificationEvent>(
                    message: string.Format(_translator[$"{Prefix}_SelfHeal_Failure"], destinationPath, ex.Message)
                );
            }
            catch (Exception publishEx) {
                CommonLog.Error(_logger, $"Self-heal warning publish failed for '{destinationPath}': {publishEx.Message}", publishEx, tag: GetType().Tag);
            }
        }
    }

    private static string BuildOldSiblingPath(string originalPath, DateTimeOffset timestamp) {
        var parentDirectoryPath = SysPath.GetDirectoryName(originalPath);

        Throws.When.NullOrWhiteSpace(parentDirectoryPath);

        var siblingName = string.Format(Constants.SystemUpdate.OldSuffixPattern, SysPath.GetFileName(originalPath), timestamp);

        return SysPath.Combine(parentDirectoryPath, siblingName);
    }

    private void VerifyInstallation(AvailableRelease release) {
        if (!SysFile.Exists(_environment.ExecutablePath)) {
            throw new InvalidDataException("The executable is missing after the update swap.");
        }

        var productVersion = FileVersionInfo.GetVersionInfo(_environment.ExecutablePath).ProductVersion;

        if (!SemVersion.TryParse(productVersion, out var parsed) || parsed != release.Version) {
            throw new InvalidDataException(
                $"The installed executable's product version ('{productVersion}') does not match the release version ('{release.Version}') after the swap."
            );
        }
    }

    // Walked back-to-front: later swaps are the ones most likely to be
    // sitting next to a partially-swapped entry. A per-entry failure is
    // reported and the loop keeps going — never throws.
    private async Task RollbackSwapsAsync(List<EntrySwap> swaps) {
        for (var idx = swaps.Count - 1; idx >= 0; idx--) {
            var swap = swaps[idx];

            try {
                if (swap.IsDirectory) {
                    if (SysDirectory.Exists(swap.OriginalPath)) { DeleteEntry(swap.OriginalPath, isDirectory: true); }
                } else if (SysFile.Exists(swap.OriginalPath)) {
                    DeleteEntry(swap.OriginalPath, isDirectory: false);
                }

                if (swap.BackupPath is not null) { MoveEntry(swap.BackupPath, swap.OriginalPath, swap.IsDirectory); }
            }
            catch (Exception ex) {
                // The publish itself is wrapped separately: a publisher
                // that is also down must not abort the loop and leave
                // remaining entries (possibly including the swapped exe)
                // un-rolled-back (lesson from #163's DeleteBackupSiblingsAsync).
                try {
                    await PublishStepFailureAsync("RollingBack", ex);
                }
                catch (Exception publishEx) {
                    CommonLog.Error(_logger, $"Rollback-failure notification failed for '{swap.OriginalPath}': {publishEx.Message}", publishEx, tag: GetType().Tag);
                }
            }
        }
    }

    private static void DeleteDirectoryBestEffort(string path) {
        try {
            if (SysDirectory.Exists(path)) { SysDirectory.Delete(path, recursive: true); }
        }
        catch (IOException) { /* best effort */ }
        catch (UnauthorizedAccessException) { /* best effort */ }
    }

    private static void DeleteFileBestEffort(string path) {
        try {
            if (SysFile.Exists(path)) { SysFile.Delete(path); }
        }
        catch (IOException) { /* best effort */ }
        catch (UnauthorizedAccessException) { /* best effort */ }
    }

    private Task PublishProgressAsync(string suffix) {
        return _eventPublisher.PublishInformationAsync<SystemUpdateProgressNotificationEvent>(
            message: _translator[$"{Prefix}_{suffix}"]
        );
    }

    private Task PublishStepFailureAsync(string suffix, Exception exception) {
        return _eventPublisher.PublishFailureAsync<SystemUpdateProgressNotificationEvent>(
            message: string.Format(_translator[$"{Prefix}_{suffix}_Failure"], exception.Message)
        );
    }

    private static string Prefix => $"{nameof(PerformSystemUpdateRequestHandler)}_{nameof(HandleAsync)}";
}
