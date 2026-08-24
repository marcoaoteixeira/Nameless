using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Nameless.Helpers;

namespace Nameless.Bootstrap;

/// <summary>
///     <see cref="IServiceCollection"/> extension methods.
/// </summary>
public static class ServiceCollectionExtensions {
    extension(IServiceCollection self) {
        /// <summary>
        ///     Registers the bootstrapper and its steps in the service collection.
        /// </summary>
        /// <param name="registration">Optional delegate to register steps.</param>
        /// <returns>
        ///     The current <see cref="IServiceCollection"/> so other actions can be chained.
        /// </returns>
        public IServiceCollection RegisterBootstrap(Action<BootstrapRegistration>? registration = null) {
            var settings = ActionHelper.FromDelegate(registration);
            
            self.TryAddTransient<IBootstrapper, Bootstrapper>();

            self.RegisterBootstrapperSteps(settings);

            return self;
        }
        private void RegisterBootstrapperSteps(BootstrapRegistration settings) {
            var service = typeof(IStep);
            
            var implementations = settings.UseAssemblyScan
                ? settings.ExecuteAssemblyScan<IStep>()
                : settings.Steps;

            var descriptors = implementations.Select(
                implementation => ServiceDescriptor.Transient(service, implementation)
            );

            self.TryAddEnumerable(descriptors);
        }
    }
}