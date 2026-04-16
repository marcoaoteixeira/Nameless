using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Nameless.Helpers;

namespace Nameless.WPF.DisasterRecovery;

public static class ServiceCollectionExtensions {
    extension(IServiceCollection self) {
        public IServiceCollection RegisterDisasterRecovery(Action<DisasterRecoveryRegistration> registration) {
            var settings = ActionHelper.FromDelegate(registration);

            self.TryAddEnumerable(
                descriptors: CreateDisasterRecoveryRoutineServiceDescriptors(settings)
            );

            return self;
        }
    }

    private static IEnumerable<ServiceDescriptor> CreateDisasterRecoveryRoutineServiceDescriptors(DisasterRecoveryRegistration settings) {
        var service = typeof(IDisasterRecoveryRoutine);
        var implementations = settings.UseAssemblyScan
            ? settings.ExecuteAssemblyScan(service)
            : settings.DisasterRecoveryRoutines;

        return implementations.Select(
            implementation => ServiceDescriptor.Transient(service, implementation)
        );
    }
}