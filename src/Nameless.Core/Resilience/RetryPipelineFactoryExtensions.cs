namespace Nameless.Resilience;

public static class RetryPipelineFactoryExtensions {
    extension(IRetryPipelineFactory self) {
        public IRetryPipeline Create(Action<Exception?, TimeSpan, int, int> onRetry) {
            return self.Create(RetryPolicyConfiguration.Default with {
                OnRetry = onRetry
            });
        }
    }
}
