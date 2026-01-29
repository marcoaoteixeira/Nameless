namespace Nameless.Bootstrap;

/// <summary>
///     Provides a base class for defining a step within the bootstrapping
///     execution pipeline.
/// </summary>
public abstract class StepBase : IStep {
    /// <inheritdoc />
    public virtual string Name => GetType().Name;

    /// <inheritdoc />
    public virtual bool ThrowOnError => false;

    /// <inheritdoc />
    public abstract Task ExecuteAsync(FlowContext context, CancellationToken cancellationToken);
}