using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Nameless.Helpers;

namespace Nameless.Workers;

public static class ServiceCollectionExtensions {
    extension(IServiceCollection self) {
        public IServiceCollection RegisterWorkers(Action<WorkersRegistration>? registration = null) {
            var settings = ActionHelper.FromDelegate(registration);

            self.TryAddEnumerable(
                descriptors: CreateWorkerServiceDescriptors(settings)
            );

            return self;
        }
    }

    private static IEnumerable<ServiceDescriptor> CreateWorkerServiceDescriptors(WorkersRegistration settings) {
        var implementations = settings.UseAssemblyScan
            ? settings.ExecuteAssemblyScan<Worker>()
            : settings.Workers;

        return implementations.Select(
            implementation => ServiceDescriptor.Singleton(
                typeof(IHostedService),
                implementation
            )
        );
    }
}