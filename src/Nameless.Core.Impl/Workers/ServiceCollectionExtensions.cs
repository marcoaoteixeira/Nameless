using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Nameless.Helpers;

namespace Nameless.Workers;

/// <summary>
///     <see cref="IServiceCollection"/> extension methods for background worker services.
/// </summary>
public static class ServiceCollectionExtensions {
    /// <param name="self">The current <see cref="IServiceCollection"/>.</param>
    extension(IServiceCollection self) {
        /// <summary>
        ///     Registers all configured <see cref="Worker"/> types as <see cref="IHostedService"/> singletons.
        /// </summary>
        /// <param name="registration">Optional delegate to configure worker types and assembly scanning.</param>
        /// <returns>
        ///     The current <see cref="IServiceCollection"/> so other actions can be chained.
        /// </returns>
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