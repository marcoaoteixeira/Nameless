using Nameless.Bootstrap.Infrastructure;
using Nameless.Bootstrap.Notification;

namespace Nameless.Bootstrap;

/// <summary>
///     Provides a base class for defining a step within the bootstrapping
///     execution pipeline.
/// </summary>
public abstract class StepBase : IStep {
    /// <inheritdoc />
    public virtual string Name => GetType().Name;

    /// <inheritdoc />
    public virtual bool IsEnabled { get; }

    /// <inheritdoc />
    public virtual IReadOnlyCollection<string> Dependencies => [];

    /// <inheritdoc />
    public virtual RetryPolicyConfiguration? RetryPolicy => null;

    /// <summary>
    ///     Initializes a new instance of <see cref="StepBase"/> class.
    /// </summary>
    /// <param name="isEnabled">
    ///     Whether it is enabled or not.
    /// </param>
    protected StepBase(bool isEnabled = true) {
        IsEnabled = isEnabled;
    }
    
    /// <inheritdoc />
    public abstract Task ExecuteAsync(FlowContext context, IProgress<StepProgress> progress, CancellationToken cancellationToken);
}