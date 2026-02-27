using Nameless.Bootstrap.Infrastructure;
using Polly;

namespace Nameless.Bootstrap.Resilience;

public interface IRetryPolicyFactory {
    ResiliencePipeline CreateRetryPipeline(string stepName, RetryPolicyConfiguration configuration);
}