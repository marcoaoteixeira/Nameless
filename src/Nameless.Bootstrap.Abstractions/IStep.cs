using Nameless.Bootstrap.Infrastructure;
using Nameless.Bootstrap.Notification;

namespace Nameless.Bootstrap;

/// <summary>
///     Represents a single step in the bootstrap process.
/// </summary>
public interface IStep {
    /// <summary>
    ///     Gets the name of the step.
    /// </summary>
    string Name { get; }

    /// <summary>
    ///     Whether it should execute the step or not.
    /// </summary>
    bool IsEnabled { get; }

    /// <summary>
    ///     Gets a list of step names that this step depends on.
    /// </summary>
    IReadOnlyCollection<string> Dependencies { get; }

    /// <summary>
    ///     Gets the retry configuration policy for the step.
    /// </summary>
    /// <remarks>
    ///     If is <see langword="null"/>, "No retry".
    /// </remarks>
    RetryPolicyConfiguration? RetryPolicy { get; }

    /// <summary>
    ///     Asynchronously executes the step logic using
    ///     the specified flow context.
    /// </summary>
    /// <param name="context">
    ///     The context object that provides data and state information for
    ///     the current step execution.
    /// </param>
    /// <param name="progress">
    ///     The progress notifier.
    /// </param>
    /// <param name="cancellationToken">
    ///     A cancellation token that can be used to cancel the asynchronous
    ///     operation.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous operation.
    /// </returns>
    Task ExecuteAsync(FlowContext context, IProgress<StepProgress> progress, CancellationToken cancellationToken);
}