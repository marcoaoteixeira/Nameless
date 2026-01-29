using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Nameless.Bootstrap;

/// <summary>
///     <see cref="IServiceCollection"/> extension methods.
/// </summary>
public static class ServiceCollectionExtensions {
    extension(IServiceCollection self) {
        /// <summary>
        ///     Registers the bootstrapping services and steps with the
        ///     current <see cref="IServiceCollection"/> instance.
        /// </summary>
        /// <param name="configure">
        ///     An optional delegate to configure
        ///     <see cref="BootstrapOptions"/> before registration.
        /// </param>
        /// <returns>
        ///     The current <see cref="IServiceCollection"/> instance so other
        ///     actions can be chained.
        /// </returns>
        public IServiceCollection RegisterBootstrap(Action<BootstrapOptions>? configure = null) {
            var innerConfigure = configure ?? (_ => { });
            var options = new BootstrapOptions();

            innerConfigure(options);

            self.TryAddSingleton<IBootstrapper, Bootstrapper>();
            self.TryAddEnumerable(CreateServiceDescriptors(options.Steps));

            return self;

            static IEnumerable<ServiceDescriptor> CreateServiceDescriptors(IEnumerable<Type> steps) {
                return steps.Select(step => ServiceDescriptor.Singleton(typeof(IStep), step));
            }
        }
    }
}