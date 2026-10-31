namespace Nameless.Windows.TaskRunner;

/// <summary>
///     Represents how a <see cref="ITaskRunnerBuilder.RunAsync"/> call ended.
/// </summary>
public enum TaskRunnerOutcome {
    /// <summary>
    ///     The delegate ran to completion (or there was no delegate to run).
    /// </summary>
    Completed,

    /// <summary>
    ///     The user cancelled the run, or the window was torn down before
    ///     or while the delegate was running.
    /// </summary>
    Cancelled
}
