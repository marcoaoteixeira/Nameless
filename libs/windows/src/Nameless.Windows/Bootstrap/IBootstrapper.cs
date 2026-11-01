using Nameless.Windows.Bootstrap.Notification;

namespace Nameless.Windows.Bootstrap;

/// <summary>
///     Defines a contract for performing application startup or initialization
///     logic asynchronously.
/// </summary>
public interface IBootstrapper {
    /// <summary>
    ///     Asynchronously executes the bootstrap operation.
    /// </summary>
    /// <param name="progress">
    ///     The progress notifier.
    /// </param>
    /// <param name="cancellationToken">
    ///     A cancellation token that can be used to cancel the operation.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous execution operation.
    /// </returns>
    Task RunAsync(IProgress<StepProgress> progress, CancellationToken cancellationToken);
}