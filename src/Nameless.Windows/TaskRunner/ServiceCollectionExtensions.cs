using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Nameless.Windows.TaskRunner.Impl;

namespace Nameless.Windows.TaskRunner;

public static class ServiceCollectionExtensions {
    extension(IServiceCollection self) {
        public IServiceCollection RegisterTaskRunner() {
            self.TryAddSingleton<ITaskRunner, TaskRunnerImpl>();

            return self;
        }
    }
}
