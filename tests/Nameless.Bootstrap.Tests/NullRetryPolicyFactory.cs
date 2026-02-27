using Nameless.Bootstrap.Infrastructure;
using Nameless.Bootstrap.Resilience;
using Polly;

namespace Nameless.Bootstrap;

public sealed class NullRetryPolicyFactory : IRetryPolicyFactory {
    public static IRetryPolicyFactory Instance { get; } = new NullRetryPolicyFactory();

    static NullRetryPolicyFactory() { }

    private NullRetryPolicyFactory() { }

    public ResiliencePipeline CreateRetryPipeline(string stepName, RetryPolicyConfiguration configuration) {
        return ResiliencePipeline.Empty;
    }
}
