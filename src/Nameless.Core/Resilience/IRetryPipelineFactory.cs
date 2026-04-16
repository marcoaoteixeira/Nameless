namespace Nameless.Resilience;

public interface IRetryPipelineFactory {
    IRetryPipeline Create(RetryPolicyConfiguration configuration);
}