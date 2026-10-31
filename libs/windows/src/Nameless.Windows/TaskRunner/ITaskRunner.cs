namespace Nameless.Windows.TaskRunner;

/// <summary>
///     Task Runner Contract
/// </summary>
public interface ITaskRunner {
    /// <summary>
    ///     Creates a new <see cref="TaskRunnerBuilder"/>.
    /// </summary>
    /// <returns>
    ///     A <see cref="TaskRunnerBuilder"/> instance.
    /// </returns>
    TaskRunnerBuilder CreateBuilder();
}