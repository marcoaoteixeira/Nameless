using Nameless.Bootstrap.Notification;
using Nameless.Resilience;

namespace Nameless.Bootstrap;

/// <summary>
///     Represents a single step in the bootstrap process.
/// </summary>
public interface IStep {
    /// <summary>
    ///     Gets the step name.
    /// </summary>
    string Name { get; }

    /// <summary>
    ///     Gets the step display name.
    /// </summary>
    string DisplayName { get; }

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
    /// <param name="progress">
    ///     The progress reporter.
    /// </param>
    /// <param name="cancellationToken">
    ///     A cancellation token that can be used to cancel the asynchronous
    ///     operation.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous operation.
    /// </returns>
    Task ExecuteAsync(IProgress<StepProgress> progress, CancellationToken cancellationToken);
}