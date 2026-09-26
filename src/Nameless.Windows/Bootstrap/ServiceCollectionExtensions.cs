using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Nameless.Helpers;

namespace Nameless.Windows.Bootstrap;

/// <summary>
///     <see cref="IServiceCollection"/> extension methods.
/// </summary>
public static class ServiceCollectionExtensions {
    extension(IServiceCollection self) {
        /// <summary>
        ///     Registers the bootstrapper and its steps in the service collection.
        /// </summary>
        /// <param name="configure">Optional delegate to register steps.</param>
        /// <returns>
        ///     The current <see cref="IServiceCollection"/> so other actions can be chained.
        /// </returns>
        public IServiceCollection RegisterBootstrap(Action<BootstrapRegistration>? configure = null) {
            var registration = ActionHelper.FromDelegate(configure);

            self.TryAddTransient<IBootstrapper, Bootstrapper>();

            self.RegisterBootstrapperSteps(registration);

            return self;
        }
        private void RegisterBootstrapperSteps(BootstrapRegistration registration) {
            var service = typeof(IStep);

            var implementations = registration.UseAssemblyScan
                ? registration.GetImplementations<IStep>()
                : registration.Steps;

            var descriptors = implementations.Select(
                implementation => ServiceDescriptor.Transient(service, implementation)
            );

            self.TryAddEnumerable(descriptors);
        }
    }
}