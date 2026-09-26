namespace Nameless.Resilience;

/// <summary>
///     Null-Object pattern implementation for <see cref="IRetryPipelineFactory"/>.
/// </summary>
public sealed class NullRetryPipelineFactory : IRetryPipelineFactory {
    /// <summary>
    ///     Gets the current single instance of <see cref="NullRetryPipelineFactory"/>.
    /// </summary>
    public static IRetryPipelineFactory Instance { get; } = new NullRetryPipelineFactory();

    static NullRetryPipelineFactory() { }

    private NullRetryPipelineFactory() { }

    /// <inheritdoc />
    public IRetryPipeline Create(RetryPolicyConfiguration configuration) {
        return RetryPipeline.Empty;
    }
}
