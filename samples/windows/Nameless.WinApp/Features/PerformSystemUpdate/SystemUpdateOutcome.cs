namespace Nameless.WinApp.Features.PerformSystemUpdate;

/// <summary>
///     Represents how a <see cref="PerformSystemUpdateRequestHandler"/> run ended.
/// </summary>
public enum SystemUpdateOutcome {
    /// <summary>
    ///     The update completed successfully; the new files are in place and
    ///     an application restart is required before they can be used.
    /// </summary>
    Completed,

    /// <summary>
    ///     The update failed; the original files were moved back and the
    ///     application can keep running as it was before the attempt.
    /// </summary>
    RolledBack
}
