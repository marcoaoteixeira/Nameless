using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Nameless.Resilience;

public static class ServiceCollectionExtensions {
    extension(IServiceCollection self) {
        public IServiceCollection RegisterRetryPipelineFactory() {
            self.TryAddTransient<IRetryPipelineFactory, RetryPipelineFactory>();

            return self;
        }
    }
}
