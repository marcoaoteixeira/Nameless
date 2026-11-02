namespace Nameless.WinApp.Features.PerformApplicationRestore;

/// <summary>
///     Represents how a <see cref="PerformApplicationRestoreRequestHandler"/> run ended.
/// </summary>
public enum RestoreOutcome {
    /// <summary>
    ///     The restore completed successfully; the application files were
    ///     swapped and an application restart is required before they can
    ///     be used.
    /// </summary>
    Completed,

    /// <summary>
    ///     The restore failed; the original files were moved back and the
    ///     application can keep running as it was before the attempt.
    /// </summary>
    RolledBack
}
