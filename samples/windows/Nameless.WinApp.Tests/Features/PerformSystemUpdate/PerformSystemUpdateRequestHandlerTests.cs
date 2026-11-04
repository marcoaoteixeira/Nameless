using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Security.Cryptography;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Nameless.IO.Physical;
using Nameless.Lucene;
using Nameless.ObjectModel;
using Nameless.Testing.Tools.IO;
using Nameless.WinApp.Data;
using Nameless.WinApp.Features.CheckSystemUpdate;
using Nameless.WinApp.Infrastructure;
using Nameless.WinApp.Infrastructure.GitHub;
using Nameless.WinApp.Tests.Tooling;
using Nameless.Windows;
using Nameless.Windows.Events;
using Nameless.Windows.Localization;
using Nameless.Windows.Notification;
using Nameless.Windows.Notification.SnackBar;

namespace Nameless.WinApp.Features.PerformSystemUpdate;

public class PerformSystemUpdateRequestHandlerTests {
    private const string ExecutableName = "Nameless.WinApp.exe";

    private sealed class Sut : IDisposable {
        public required TempDirectory InstallRoot { get; init; }
        public required TempDirectory AppDataRoot { get; init; }
        public required PhysicalFileManager FileManager { get; init; }
        public required FakeSystemUpdateEnvironment Environment { get; init; }
        public required Mock<IGitHubReleaseClient> GitHubReleaseClientMock { get; init; }
        public required Mock<IEventPublisher> EventPublisherMock { get; init; }
        public required AppDbContext DbContext { get; init; }
        public required PerformSystemUpdateRequestHandler Handler { get; init; }
        public required AvailableRelease Release { get; init; }
        public required string PackagePath { get; init; }

        public void Dispose() {
            DbContext.Dispose();
            InstallRoot.Dispose();
            AppDataRoot.Dispose();
        }
    }

    // The test project's own apphost is a real PE with a real, parseable
    // ProductVersion ("1.0.0+<sha>" — Directory.Build.props pins
    // AssemblyVersion to 1.0.0 and the test project does not disable
    // source-revision suffixing). SemVersion equality ignores build
    // metadata, so comparisons against SemVersion.Parse("1.0.0") succeed —
    // this lets tests exercise the real FileVersionInfo-based checks
    // without crafting a synthetic PE.
    //
    // This is derived from the test assembly's own location rather than
    // Environment.ProcessPath: `dotnet test` (pinned to Microsoft.Testing.
    // Platform via global.json) launches this project's own apphost
    // directly, so ProcessPath happens to line up, but Visual Studio/Rider
    // Test Explorer launch the test host via the `dotnet` muxer against the
    // managed dll, where ProcessPath resolves to dotnet.exe — a PE stamped
    // with the installed SDK/runtime version, not 1.0.0 (#173).
    private static string CurrentTestExecutablePath =>
        SysPath.ChangeExtension(Assembly.GetExecutingAssembly().Location, ".exe");

    private static byte[] BuildPackageZip(string executableSourcePath, Action<string>? populateExtra = null) {
        using var staging = new TempDirectory(nameof(PerformSystemUpdateRequestHandlerTests));

        SysFile.Copy(executableSourcePath, SysPath.Combine(staging.Path, ExecutableName));

        populateExtra?.Invoke(staging.Path);

        var zipPath = SysPath.Combine(SysPath.GetTempPath(), $"{Guid.NewGuid():N}.zip");

        try {
            ZipFile.CreateFromDirectory(staging.Path, zipPath);

            return SysFile.ReadAllBytes(zipPath);
        }
        finally {
            if (SysFile.Exists(zipPath)) { SysFile.Delete(zipPath); }
        }
    }

    private static string ComputeSha256Hex(byte[] content) {
        return Convert.ToHexStringLower(SHA256.HashData(content));
    }

    private static Sut CreateSut(
        SemVersion? releaseVersion = null,
        byte[]? packageBytesOverride = null,
        string? checksumTextOverride = null,
        long? declaredPackageSizeOverride = null,
        bool isSelfUpdateEnabled = true) {
        var installRoot = new TempDirectory(nameof(PerformSystemUpdateRequestHandlerTests) + "-install");
        var appDataRoot = new TempDirectory(nameof(PerformSystemUpdateRequestHandlerTests) + "-appdata");

        var executablePath = SysPath.Combine(installRoot.Path, ExecutableName);
        SysFile.Copy(CurrentTestExecutablePath, executablePath);

        // Entries the package never contains and the update must never touch.
        var logsDirectory = SysPath.Combine(installRoot.Path, "logs");
        SysDirectory.CreateDirectory(logsDirectory);
        SysFile.WriteAllText(SysPath.Combine(logsDirectory, "app.log"), "untouched-log");

        var version = releaseVersion ?? SemVersion.Parse("1.0.0");

        var packageBytes = packageBytesOverride ?? BuildPackageZip(CurrentTestExecutablePath);
        var packageHash = ComputeSha256Hex(packageBytes);
        var checksumText = checksumTextOverride ?? $"{packageHash}  Nameless.WinApp-{version.Format()}-win-x64.zip";

        var packageName = $"Nameless.WinApp-{version.Format()}-win-x64.zip";
        var packageUrl = new Uri($"https://github.com/marcoaoteixeira/Nameless/releases/download/winapp-v{version.Format()}/{packageName}");
        var checksumUrl = new Uri($"{packageUrl}.sha256");

        var declaredSize = declaredPackageSizeOverride ?? packageBytes.LongLength;

        var release = new AvailableRelease(
            Version: version,
            TagName: $"winapp-v{version.Format()}",
            HtmlUrl: new Uri($"https://github.com/marcoaoteixeira/Nameless/releases/tag/winapp-v{version.Format()}"),
            Package: new ReleaseAsset(packageName, declaredSize, packageUrl),
            Checksum: new ReleaseAsset($"{packageName}.sha256", checksumText.Length, checksumUrl)
        );

        var fileManager = new PhysicalFileManager(appDataRoot.Path);
        var packagePath = SysPath.Combine(fileManager.GetUpdateDirectory().Path, packageName);

        var gitHubReleaseClientMock = new Mock<IGitHubReleaseClient>();
        gitHubReleaseClientMock
            .Setup(x => x.DownloadTextAsync(checksumUrl, It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .Returns<Uri, long, CancellationToken>((_, _, ct) => {
                ct.ThrowIfCancellationRequested();

                return Task.FromResult(checksumText);
            });
        gitHubReleaseClientMock
            .Setup(x => x.DownloadToFileAsync(packageUrl, It.IsAny<string>(), release.Package.Size, It.IsAny<CancellationToken>()))
            .Returns<Uri, string, long, CancellationToken>(async (_, destinationPath, _, ct) => {
                var directoryPath = SysPath.GetDirectoryName(destinationPath);

                if (directoryPath is not null) { SysDirectory.CreateDirectory(directoryPath); }

                await SysFile.WriteAllBytesAsync(destinationPath, packageBytes, ct);
            });

        var environment = new FakeSystemUpdateEnvironment {
            InstallDirectoryPath = installRoot.Path,
            ExecutablePath = executablePath,
            IsSelfUpdateEnabled = isSelfUpdateEnabled
        };

        var indexProviderMock = new Mock<IIndexProvider>();

        var eventPublisherMock = new Mock<IEventPublisher>();
        eventPublisherMock
            .Setup(x => x.PublishAsync(It.IsAny<SystemUpdateProgressNotificationEvent>()))
            .Returns(Task.CompletedTask);
        eventPublisherMock
            .Setup(x => x.PublishAsync(It.IsAny<SnackBarNotificationEvent>()))
            .Returns(Task.CompletedTask);

        var dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite("Data Source=:memory:")
            .Options;
        var dbContext = new AppDbContext(dbContextOptions);

        var handler = new PerformSystemUpdateRequestHandler(
            gitHubReleaseClient: gitHubReleaseClientMock.Object,
            environment: environment,
            fileManager: fileManager,
            dbContext: dbContext,
            indexProvider: indexProviderMock.Object,
            eventPublisher: eventPublisherMock.Object,
            translator: new FakeTranslator(),
            logger: NullLogger<PerformSystemUpdateRequestHandler>.Instance
        );

        return new Sut {
            InstallRoot = installRoot,
            AppDataRoot = appDataRoot,
            FileManager = fileManager,
            Environment = environment,
            GitHubReleaseClientMock = gitHubReleaseClientMock,
            EventPublisherMock = eventPublisherMock,
            DbContext = dbContext,
            Handler = handler,
            Release = release,
            PackagePath = packagePath
        };
    }

    [Fact]
    [UnitTest]
    public async Task HandleAsync_WhenRequestIsNull_ThrowsArgumentNullException() {
        using var sut = CreateSut();

        await Assert.ThrowsAsync<ArgumentNullException>(
            () => sut.Handler.HandleAsync(null!, TestContext.Current.CancellationToken)
        );
    }

    [Fact]
    [UnitTest]
    public async Task HandleAsync_HappyPath_SwapsExecutableLeavesUnrelatedEntriesAloneAndCleansUpStaging() {
        using var sut = CreateSut();

        var request = new PerformSystemUpdateRequest(sut.Release);

        var outcome = await sut.Handler.HandleAsync(request, TestContext.Current.CancellationToken);

        Assert.Equal(SystemUpdateOutcome.Completed, outcome);

        // The new exe (a copy of the live test host) is in place.
        Assert.True(SysFile.Exists(sut.Environment.ExecutablePath));

        // The old exe was renamed aside, not deleted.
        var oldSiblings = SysDirectory.GetFileSystemEntries(sut.InstallRoot.Path)
            .Where(entry => SysPath.GetFileName(entry).Contains(".old-", StringComparison.Ordinal))
            .ToArray();
        Assert.Single(oldSiblings);

        // Entries the package never touched are exactly as they were.
        Assert.Equal("untouched-log", SysFile.ReadAllText(SysPath.Combine(sut.InstallRoot.Path, "logs", "app.log")));

        // Staging and the downloaded package are cleaned up.
        Assert.DoesNotContain(SysDirectory.GetFileSystemEntries(sut.InstallRoot.Path), entry => SysPath.GetFileName(entry).StartsWith(".update-", StringComparison.Ordinal));
        Assert.False(SysFile.Exists(sut.PackagePath));

        sut.EventPublisherMock.Verify(
            x => x.PublishAsync(It.Is<SystemUpdateProgressNotificationEvent>(e => e.Message.Contains("_Success", StringComparison.Ordinal))),
            Times.Once
        );
        sut.EventPublisherMock.Verify(
            x => x.PublishAsync(It.Is<SnackBarNotificationEvent>(e => e.Type == NotificationType.Failure)),
            Times.Never
        );
    }

    [Fact]
    [UnitTest]
    public async Task HandleAsync_WhenSelfUpdateMarkerIsAbsent_FailsAtPrerequisitesWithNothingTouched() {
        using var sut = CreateSut(isSelfUpdateEnabled: false);

        var originalExeBytes = SysFile.ReadAllBytes(sut.Environment.ExecutablePath);

        var outcome = await sut.Handler.HandleAsync(new PerformSystemUpdateRequest(sut.Release), TestContext.Current.CancellationToken);

        Assert.Equal(SystemUpdateOutcome.RolledBack, outcome);
        Assert.Equal(originalExeBytes, SysFile.ReadAllBytes(sut.Environment.ExecutablePath));

        // Never even reached the network: the GitHub client was never called.
        sut.GitHubReleaseClientMock.Verify(x => x.DownloadTextAsync(It.IsAny<Uri>(), It.IsAny<long>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    [UnitTest]
    public async Task HandleAsync_WhenChecksumSidecarDoesNotMatchDownloadedPackage_RollsBackAtTheHashCheckAndRemovesPartialPackage() {
        var version = SemVersion.Parse("1.0.0");
        var packageName = $"Nameless.WinApp-{version.Format()}-win-x64.zip";

        // Correct name token, wrong hash: this must fail at the hash
        // comparison (handler's VerifyPackageAsync), not at the sidecar's
        // earlier name-token check.
        using var sut = CreateSut(checksumTextOverride: $"{new string('a', 64)}  {packageName}");

        var outcome = await sut.Handler.HandleAsync(new PerformSystemUpdateRequest(sut.Release), TestContext.Current.CancellationToken);

        Assert.Equal(SystemUpdateOutcome.RolledBack, outcome);
        Assert.False(SysFile.Exists(sut.PackagePath));

        sut.EventPublisherMock.Verify(
            x => x.PublishAsync(It.Is<SystemUpdateProgressNotificationEvent>(e => e.Message.Contains("VerifyingPackage_Failure", StringComparison.Ordinal))),
            Times.Once
        );

        // Nothing was swapped: the original exe is untouched.
        Assert.DoesNotContain(SysDirectory.GetFileSystemEntries(sut.InstallRoot.Path), entry => SysPath.GetFileName(entry).Contains(".old-", StringComparison.Ordinal));
    }

    [Fact]
    [UnitTest]
    public async Task HandleAsync_WhenChecksumSidecarNameTokenDoesNotMatchThePackageName_RollsBackAtTheSidecarParseStep() {
        using var sut = CreateSut(checksumTextOverride: $"{new string('a', 64)}  whatever.zip");

        var outcome = await sut.Handler.HandleAsync(new PerformSystemUpdateRequest(sut.Release), TestContext.Current.CancellationToken);

        Assert.Equal(SystemUpdateOutcome.RolledBack, outcome);
        Assert.False(SysFile.Exists(sut.PackagePath));

        sut.EventPublisherMock.Verify(
            x => x.PublishAsync(It.Is<SystemUpdateProgressNotificationEvent>(e => e.Message.Contains("DownloadingChecksum_Failure", StringComparison.Ordinal))),
            Times.Once
        );
    }

    // Plan §8's documented sidecar edge cases (ParseChecksumSidecar,
    // handler :230-251). A regression in a *positive* case here would
    // break every real update, since GNU sha256sum output legitimately
    // varies in case, BOM presence and trailing newline.
    [Theory]
    [UnitTest]
    [InlineData("valid-uppercase-hash", true)]
    [InlineData("valid-bom-prefix", true)]
    [InlineData("valid-no-trailing-newline", true)]
    [InlineData("malformed-63-char-hash", false)]
    [InlineData("malformed-65-char-hash", false)]
    [InlineData("malformed-non-hex", false)]
    [InlineData("malformed-empty", false)]
    [InlineData("malformed-wrong-name-token", false)]
    public async Task HandleAsync_ChecksumSidecarEdgeCases_ParsesPerContractOrRollsBack(string variant, bool expectCompleted) {
        var version = SemVersion.Parse("1.0.0");
        var packageName = $"Nameless.WinApp-{version.Format()}-win-x64.zip";
        var packageBytes = BuildPackageZip(CurrentTestExecutablePath);
        var hash = ComputeSha256Hex(packageBytes);

        var checksumText = variant switch {
            "valid-uppercase-hash" => $"{hash.ToUpperInvariant()}  {packageName}",
            "valid-bom-prefix" => $"﻿{hash}  {packageName}",
            "valid-no-trailing-newline" => $"{hash}  {packageName}",
            "malformed-63-char-hash" => $"{hash[..63]}  {packageName}",
            "malformed-65-char-hash" => $"{hash}a  {packageName}",
            "malformed-non-hex" => $"{new string('g', 64)}  {packageName}",
            "malformed-empty" => string.Empty,
            "malformed-wrong-name-token" => $"{hash}  whatever.zip",
            _ => throw new ArgumentOutOfRangeException(nameof(variant))
        };

        using var sut = CreateSut(packageBytesOverride: packageBytes, checksumTextOverride: checksumText);

        var outcome = await sut.Handler.HandleAsync(new PerformSystemUpdateRequest(sut.Release), TestContext.Current.CancellationToken);

        if (expectCompleted) {
            Assert.Equal(SystemUpdateOutcome.Completed, outcome);
        } else {
            Assert.Equal(SystemUpdateOutcome.RolledBack, outcome);

            sut.EventPublisherMock.Verify(
                x => x.PublishAsync(It.Is<SystemUpdateProgressNotificationEvent>(e => e.Message.Contains("DownloadingChecksum_Failure", StringComparison.Ordinal))),
                Times.Once
            );
        }
    }

    [Fact]
    [UnitTest]
    public async Task HandleAsync_WhenDownloadedLengthDoesNotMatchDeclaredSize_RollsBackAndRemovesPartialPackage() {
        var packageBytes = BuildPackageZip(CurrentTestExecutablePath);

        using var sut = CreateSut(
            packageBytesOverride: packageBytes,
            declaredPackageSizeOverride: packageBytes.LongLength + 1024
        );

        var outcome = await sut.Handler.HandleAsync(new PerformSystemUpdateRequest(sut.Release), TestContext.Current.CancellationToken);

        Assert.Equal(SystemUpdateOutcome.RolledBack, outcome);
        Assert.False(SysFile.Exists(sut.PackagePath));
    }

    [Fact]
    [UnitTest]
    public async Task HandleAsync_WhenPackageContainsForbiddenRootEntry_RollsBackWithoutSwappingAnything() {
        var packageBytes = BuildPackageZip(CurrentTestExecutablePath, stagingPath => {
            var forbidden = SysPath.Combine(stagingPath, "logs");
            SysDirectory.CreateDirectory(forbidden);
            SysFile.WriteAllText(SysPath.Combine(forbidden, "malicious.log"), "nope");
        });

        using var sut = CreateSut(packageBytesOverride: packageBytes);

        var originalExeBytes = SysFile.ReadAllBytes(sut.Environment.ExecutablePath);

        var outcome = await sut.Handler.HandleAsync(new PerformSystemUpdateRequest(sut.Release), TestContext.Current.CancellationToken);

        Assert.Equal(SystemUpdateOutcome.RolledBack, outcome);
        Assert.Equal(originalExeBytes, SysFile.ReadAllBytes(sut.Environment.ExecutablePath));
        Assert.DoesNotContain(SysDirectory.GetFileSystemEntries(sut.InstallRoot.Path), entry => SysPath.GetFileName(entry).Contains(".old-", StringComparison.Ordinal));
    }

    [Fact]
    [UnitTest]
    public async Task HandleAsync_WhenPackageIsMissingTheExecutableAtRoot_RollsBackAtValidation() {
        using var staging = new TempDirectory(nameof(PerformSystemUpdateRequestHandlerTests));
        SysFile.WriteAllText(SysPath.Combine(staging.Path, "appsettings.json"), "{}");

        var zipPath = SysPath.Combine(SysPath.GetTempPath(), $"{Guid.NewGuid():N}.zip");
        ZipFile.CreateFromDirectory(staging.Path, zipPath);
        var packageBytes = SysFile.ReadAllBytes(zipPath);
        SysFile.Delete(zipPath);

        using var sut = CreateSut(packageBytesOverride: packageBytes);

        var outcome = await sut.Handler.HandleAsync(new PerformSystemUpdateRequest(sut.Release), TestContext.Current.CancellationToken);

        Assert.Equal(SystemUpdateOutcome.RolledBack, outcome);
    }

    [Fact]
    [UnitTest]
    public async Task HandleAsync_WhenStagedExecutableVersionDoesNotMatchReleaseVersion_RollsBackAtValidation() {
        // The copied test host exe's real ProductVersion is "1.0.0+<sha>";
        // declaring the release as a different version makes the
        // post-extraction version check fail deterministically.
        using var sut = CreateSut(releaseVersion: SemVersion.Parse("9.9.9"));

        var outcome = await sut.Handler.HandleAsync(new PerformSystemUpdateRequest(sut.Release), TestContext.Current.CancellationToken);

        Assert.Equal(SystemUpdateOutcome.RolledBack, outcome);
    }

    [Fact]
    [UnitTest]
    public async Task HandleAsync_WhenRenameAsideOfTheExecutableFails_RollsBackTheEarlierAlreadySwappedEntry() {
        var packageBytes = BuildPackageZip(CurrentTestExecutablePath, stagingPath =>
            SysFile.WriteAllText(SysPath.Combine(stagingPath, "appsettings.json"), "{\"new\":true}"));

        using var sut = CreateSut(packageBytesOverride: packageBytes);

        SysFile.WriteAllText(SysPath.Combine(sut.InstallRoot.Path, "appsettings.json"), "{\"old\":true}");

        // Lock the live exe open without share-delete: the handler's
        // SwappingFiles step processes the exe last, so by the time it
        // tries to rename the original exe aside (the move OUT of
        // destinationPath, outside SwapEntryAsync's try block),
        // appsettings.json has already swapped successfully — exercising
        // the outer rollback of an earlier, already-succeeded entry. This
        // fails before SwapEntryAsync's try block even starts, so it never
        // reaches self-heal.
        using var exeLock = new FileStream(sut.Environment.ExecutablePath, FileMode.Open, FileAccess.Read, FileShare.Read);

        var outcome = await sut.Handler.HandleAsync(new PerformSystemUpdateRequest(sut.Release), TestContext.Current.CancellationToken);

        Assert.Equal(SystemUpdateOutcome.RolledBack, outcome);
        Assert.Equal("{\"old\":true}", SysFile.ReadAllText(SysPath.Combine(sut.InstallRoot.Path, "appsettings.json")));
        Assert.DoesNotContain(SysDirectory.GetFileSystemEntries(sut.InstallRoot.Path), entry => SysPath.GetFileName(entry).Contains(".old-", StringComparison.Ordinal));
    }

    // Test-only subclass: throws specifically on the move INTO the
    // executable's destination path (i.e. after the rename-aside already
    // succeeded, inside SwapEntryAsync's try block), to deterministically
    // exercise the self-heal branch (handler :370-373) without relying on
    // file locks or timing.
    private sealed class SelfHealTriggeringHandler : PerformSystemUpdateRequestHandler {
        private readonly string _failingDestinationPath;
        private int _moveIntoDestinationAttempts;

        public SelfHealTriggeringHandler(
            IGitHubReleaseClient gitHubReleaseClient,
            ISystemUpdateEnvironment environment,
            PhysicalFileManager fileManager,
            AppDbContext dbContext,
            IIndexProvider indexProvider,
            IEventPublisher eventPublisher,
            ITranslator translator,
            ILogger<PerformSystemUpdateRequestHandler> logger,
            string failingDestinationPath)
            : base(gitHubReleaseClient, environment, fileManager, dbContext, indexProvider, eventPublisher, translator, logger) {
            _failingDestinationPath = failingDestinationPath;
        }

        protected override void MoveEntry(string sourcePath, string destinationPath, bool isDirectory) {
            if (string.Equals(destinationPath, _failingDestinationPath, StringComparison.OrdinalIgnoreCase)) {
                _moveIntoDestinationAttempts++;

                // Only the move INTO the destination (the staged entry
                // replacing the renamed-aside original) must fail — the
                // self-heal's own move-back uses the same destination path
                // as its argument, so it must succeed.
                if (_moveIntoDestinationAttempts == 1) {
                    throw new IOException("Simulated failure moving the staged entry into place.");
                }
            }

            base.MoveEntry(sourcePath, destinationPath, isDirectory);
        }
    }

    [Fact]
    [UnitTest]
    public async Task HandleAsync_WhenMoveIntoPlaceFailsAfterRenameAside_SelfHealsTheExecutableAndRollsBack() {
        using var sut = CreateSut();

        var originalExeBytes = SysFile.ReadAllBytes(sut.Environment.ExecutablePath);

        var handler = new SelfHealTriggeringHandler(
            gitHubReleaseClient: sut.GitHubReleaseClientMock.Object,
            environment: sut.Environment,
            fileManager: sut.FileManager,
            dbContext: sut.DbContext,
            indexProvider: new Mock<IIndexProvider>().Object,
            eventPublisher: sut.EventPublisherMock.Object,
            translator: new FakeTranslator(),
            logger: NullLogger<PerformSystemUpdateRequestHandler>.Instance,
            failingDestinationPath: sut.Environment.ExecutablePath
        );

        var outcome = await handler.HandleAsync(new PerformSystemUpdateRequest(sut.Release), TestContext.Current.CancellationToken);

        Assert.Equal(SystemUpdateOutcome.RolledBack, outcome);

        // Self-heal restored the original exe at its original path...
        Assert.True(SysFile.Exists(sut.Environment.ExecutablePath));
        Assert.Equal(originalExeBytes, SysFile.ReadAllBytes(sut.Environment.ExecutablePath));

        // ...and no ".old-*" sibling is left behind.
        Assert.DoesNotContain(SysDirectory.GetFileSystemEntries(sut.InstallRoot.Path), entry => SysPath.GetFileName(entry).Contains(".old-", StringComparison.Ordinal));
    }

    // Test-only subclass: throws specifically when rolling back the
    // executable's swap, to deterministically exercise RollbackSwapsAsync's
    // per-entry catch continuing to the next entry even when that entry's
    // own failure-publish also throws (lesson from #163's
    // DeleteBackupSiblingsAsync, applied here).
    private sealed class RollbackFailureTriggeringHandler : PerformSystemUpdateRequestHandler {
        private readonly string _failingOriginalPath;

        public RollbackFailureTriggeringHandler(
            IGitHubReleaseClient gitHubReleaseClient,
            ISystemUpdateEnvironment environment,
            PhysicalFileManager fileManager,
            AppDbContext dbContext,
            IIndexProvider indexProvider,
            IEventPublisher eventPublisher,
            ITranslator translator,
            ILogger<PerformSystemUpdateRequestHandler> logger,
            string failingOriginalPath)
            : base(gitHubReleaseClient, environment, fileManager, dbContext, indexProvider, eventPublisher, translator, logger) {
            _failingOriginalPath = failingOriginalPath;
        }

        protected override void DeleteEntry(string path, bool isDirectory) {
            if (string.Equals(path, _failingOriginalPath, StringComparison.OrdinalIgnoreCase)) {
                throw new IOException("Simulated failure deleting the swapped-in entry during rollback.");
            }

            base.DeleteEntry(path, isDirectory);
        }
    }

    [Fact]
    [UnitTest]
    public async Task HandleAsync_WhenOneEntrysRollbackFailsAndItsFailurePublishAlsoFails_StillRollsBackTheOtherEntries() {
        var packageBytes = BuildPackageZip(CurrentTestExecutablePath, stagingPath =>
            SysFile.WriteAllText(SysPath.Combine(stagingPath, "appsettings.json"), "{\"new\":true}"));

        using var sut = CreateSut(packageBytesOverride: packageBytes);

        SysFile.WriteAllText(SysPath.Combine(sut.InstallRoot.Path, "appsettings.json"), "{\"old\":true}");

        // Fails rollback specifically for the exe entry (processed first,
        // being the last swap added) — both its own delete and the
        // failure-publish about that delete throw. The appsettings.json
        // entry's rollback must still run afterward.
        sut.EventPublisherMock
            .Setup(x => x.PublishAsync(It.Is<SystemUpdateProgressNotificationEvent>(e => e.Message.Contains("_VerifyingInstallation", StringComparison.Ordinal) && !e.Message.Contains("_Failure", StringComparison.Ordinal))))
            .ThrowsAsync(new InvalidOperationException("forcing rollback after both swaps succeeded"));
        sut.EventPublisherMock
            .Setup(x => x.PublishAsync(It.Is<SystemUpdateProgressNotificationEvent>(e => e.Message.Contains("RollingBack_Failure", StringComparison.Ordinal))))
            .ThrowsAsync(new InvalidOperationException("the failure-publish is also down"));

        var handler = new RollbackFailureTriggeringHandler(
            gitHubReleaseClient: sut.GitHubReleaseClientMock.Object,
            environment: sut.Environment,
            fileManager: sut.FileManager,
            dbContext: sut.DbContext,
            indexProvider: new Mock<IIndexProvider>().Object,
            eventPublisher: sut.EventPublisherMock.Object,
            translator: new FakeTranslator(),
            logger: NullLogger<PerformSystemUpdateRequestHandler>.Instance,
            failingOriginalPath: sut.Environment.ExecutablePath
        );

        var outcome = await handler.HandleAsync(new PerformSystemUpdateRequest(sut.Release), TestContext.Current.CancellationToken);

        Assert.Equal(SystemUpdateOutcome.RolledBack, outcome);

        // The exe's own rollback failed (deliberately) — it is left as
        // whatever the failed DeleteEntry left behind, not asserted here.
        // What matters is that the earlier appsettings.json entry's
        // rollback still ran to completion despite the exe entry's
        // failure-publish also throwing.
        Assert.Equal("{\"old\":true}", SysFile.ReadAllText(SysPath.Combine(sut.InstallRoot.Path, "appsettings.json")));
    }

    [Fact]
    [UnitTest]
    public async Task HandleAsync_WhenCancelledBeforeCommit_RollsBackAndPublishesCancelledMessageWithNothingTouched() {
        using var sut = CreateSut();

        var originalExeBytes = SysFile.ReadAllBytes(sut.Environment.ExecutablePath);

        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        var outcome = await sut.Handler.HandleAsync(new PerformSystemUpdateRequest(sut.Release), cts.Token);

        Assert.Equal(SystemUpdateOutcome.RolledBack, outcome);
        Assert.Equal(originalExeBytes, SysFile.ReadAllBytes(sut.Environment.ExecutablePath));

        sut.EventPublisherMock.Verify(
            x => x.PublishAsync(It.Is<SnackBarNotificationEvent>(e => e.Message.Contains("RolledBack_Cancelled", StringComparison.Ordinal))),
            Times.Once
        );
    }

    [Fact]
    [UnitTest]
    public async Task HandleAsync_WhenCancellationArrivesAtCommit_StillReturnsCompletedWithoutRollingBackSucceededWork() {
        using var sut = CreateSut();

        // Stands in for a cancellation signal arriving during the
        // post-commit tail: once committed, an OperationCanceledException
        // must not be mistaken for a reason to roll back.
        sut.EventPublisherMock
            .Setup(x => x.PublishAsync(It.Is<SystemUpdateProgressNotificationEvent>(e => e.Message.Contains("_Success", StringComparison.Ordinal))))
            .ThrowsAsync(new OperationCanceledException("cancelled after commit"));

        var outcome = await sut.Handler.HandleAsync(new PerformSystemUpdateRequest(sut.Release), TestContext.Current.CancellationToken);

        Assert.Equal(SystemUpdateOutcome.Completed, outcome);
        Assert.True(SysFile.Exists(sut.Environment.ExecutablePath));
    }

    [Fact]
    [UnitTest]
    public async Task HandleAsync_WhenSuccessPublishFailsAfterCommit_KeepsSwappedFilesAndReturnsCompleted() {
        // Lesson from #163: a failure in the tail (even a plain exception,
        // not just cancellation) must stay cosmetic once committed.
        using var sut = CreateSut();

        sut.EventPublisherMock
            .Setup(x => x.PublishAsync(It.Is<SystemUpdateProgressNotificationEvent>(e => e.Message.Contains("_Success", StringComparison.Ordinal))))
            .ThrowsAsync(new InvalidOperationException("event bus is down"));

        var outcome = await sut.Handler.HandleAsync(new PerformSystemUpdateRequest(sut.Release), TestContext.Current.CancellationToken);

        Assert.Equal(SystemUpdateOutcome.Completed, outcome);
        Assert.True(SysFile.Exists(sut.Environment.ExecutablePath));

        sut.EventPublisherMock.Verify(
            x => x.PublishAsync(It.Is<SnackBarNotificationEvent>(e => e.Type == NotificationType.Failure)),
            Times.Never
        );
    }
}
