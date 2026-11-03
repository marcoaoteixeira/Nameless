using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Nameless.Application;
using Nameless.Contracts;
using Nameless.WinApp.Infrastructure;
using Nameless.Windows;
using Nameless.Windows.Bootstrap;
using Nameless.Windows.Bootstrap.Notification;
using Nameless.Windows.Localization;

namespace Nameless.WinApp.Bootstrap.Steps;

/// <summary>
///     Deletes the leftovers a previous self-update run could not remove
///     while its own process was running (plan §3.6): the renamed-aside
///     <c>*.old-{timestamp}</c> entries, the <c>.update-*</c> staging/probe
///     names, and anything still sitting in the update download directory.
/// </summary>
/// <remarks>
///     Runs last, after every other startup step: a successful run of this
///     step is this (new) version's proof of life, which is why the
///     leftovers are kept around until now instead of being deleted right
///     after the swap commits.
/// </remarks>
public class CleanupPreviousSystemUpdateStep : StepBase {
    // Release contract-adjacent safety fence (plan §3.6, §5.3): must match
    // only a direct child literally named "{name}.old-{14 digits}" or
    // ".update-{...}{14 digits}", and nothing else — not "logs", not
    // "App_Data", not the running exe's own name, not a near-miss variant
    // with the wrong digit count or an extra suffix. Getting this wrong
    // means deleting live application files on every third startup step.
    private static readonly Regex OldSiblingPattern = new(@"^.+\.old-\d{14}$", RegexOptions.Compiled | RegexOptions.CultureInvariant);
    private static readonly Regex StagingOrProbePattern = new(@"^\.update-(probe-)?\d{14}$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private readonly ISystemUpdateEnvironment _environment;
    private readonly IApplicationContext _applicationContext;
    private readonly ILogger<CleanupPreviousSystemUpdateStep> _logger;

    private ITranslator T { get; }

    /// <inheritdoc />
    public override IReadOnlyCollection<string> Dependencies => [
        nameof(SetUILanguageStep),
        nameof(EnsureApplicationDirectoriesExistenceStep),
        nameof(InitializeLuceneIndexStep)
    ];

    /// <inheritdoc />
    public override string DisplayName => T.Get(nameof(DisplayName));

    /// <summary>
    ///     Initializes a new instance of
    ///     <see cref="CleanupPreviousSystemUpdateStep"/> class.
    /// </summary>
    /// <param name="environment">
    ///     The system update environment.
    /// </param>
    /// <param name="applicationContext">
    ///     The application context.
    /// </param>
    /// <param name="translator">
    ///     The translator service.
    /// </param>
    /// <param name="logger">
    ///     The logger.
    /// </param>
    public CleanupPreviousSystemUpdateStep(
        ISystemUpdateEnvironment environment,
        IApplicationContext applicationContext,
        ITranslator translator,
        ILogger<CleanupPreviousSystemUpdateStep> logger) {
        _environment = Throws.When.Null(environment);
        _applicationContext = Throws.When.Null(applicationContext);
        T = Throws.When.Null(translator);
        _logger = Throws.When.Null(logger);
    }

    /// <summary>
    ///     Whether <paramref name="entryName"/> is a leftover name this step
    ///     is allowed to delete (fitness function, plan §5.3): exactly
    ///     <c>"{name}.old-{14 digits}"</c> or <c>".update-{...}{14 digits}"</c>.
    /// </summary>
    /// <param name="entryName">
    ///     A direct child file or directory name (not a full path).
    /// </param>
    public static bool IsPreviousUpdateLeftover(string entryName) {
        Throws.When.Null(entryName);

        return OldSiblingPattern.IsMatch(entryName) || StagingOrProbePattern.IsMatch(entryName);
    }

    /// <inheritdoc />
    public override Task ExecuteAsync(IProgress<StepProgress> progress, CancellationToken cancellationToken) {
        progress.ReportInformation(DisplayName, T.Get("CleaningUp"));

        DeleteMatchingInstallDirectoryEntries();
        DeleteUpdateDirectoryContents();

        return Task.CompletedTask;
    }

    private void DeleteMatchingInstallDirectoryEntries() {
        foreach (var entryPath in SysDirectory.EnumerateFileSystemEntries(_environment.InstallDirectoryPath)) {
            var entryName = SysPath.GetFileName(entryPath);

            if (IsPreviousUpdateLeftover(entryName)) { DeleteBestEffort(entryPath); }
        }
    }

    private void DeleteUpdateDirectoryContents() {
        var updateDirectory = _applicationContext.FileManager.GetUpdateDirectory();

        if (!updateDirectory.Exists) { return; }

        foreach (var filePath in SysDirectory.EnumerateFileSystemEntries(updateDirectory.Path)) {
            DeleteBestEffort(filePath);
        }
    }

    // Best-effort: a leftover that is still locked (the renamed-away
    // running exe, before the old process has fully exited) is skipped and
    // logged, not thrown — the next startup gets another chance, and this
    // step must never block bootstrap.
    private void DeleteBestEffort(string path) {
        try {
            if (SysDirectory.Exists(path)) { SysDirectory.Delete(path, recursive: true); }
            else if (SysFile.Exists(path)) { SysFile.Delete(path); }
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException) {
            CommonLog.Warning(_logger, $"Could not delete self-update leftover '{path}': {ex.Message}", tag: GetType().Tag);
        }
    }
}
