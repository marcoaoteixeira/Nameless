namespace Nameless.Resilience;

/// <summary>
///     Represents a <see cref="IRetryPipeline"/> factory.
/// </summary>
public interface IRetryPipelineFactory {
    /// <summary>
    ///     Creates a new <see cref="IRetryPipeline"/> for the given
    ///     retry policy configuration.
    /// </summary>
    /// <param name="configuration">
    ///     The configuration of the pipeline.
    /// </param>
    /// <returns>
    ///     A new instance of <see cref="IRetryPipeline"/>.
    /// </returns>
    IRetryPipeline Create(RetryPolicyConfiguration configuration);
}