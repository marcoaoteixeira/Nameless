namespace Nameless.Windows.TaskRunner;

/// <summary>
///     The task runner delegate.
/// </summary>
/// <param name="cancellationToken">
///     The cancellation token.
/// </param>
/// <returns>
///     A <see cref="Task"/> representing the asynchronous method execution.
/// </returns>
public delegate Task TaskRunnerDelegate(CancellationToken cancellationToken);