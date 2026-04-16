using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Nameless.Helpers;
using Nameless.WPF.DependencyInjection;
using Nameless.WPF.Windows.Impl;

namespace Nameless.WPF.Windows;

/// <summary>
///     <see cref="IServiceCollection"/> extension methods.
/// </summary>
public static class ServiceCollectionExtensions {
    /// <param name="self">
    ///     The current <see cref="IServiceCollection"/>.
    /// </param>
    extension(IServiceCollection self) {
        /// <summary>
        ///     Registers the window services.
        /// </summary>
        /// <param name="registration">
        ///     Configure action for <see cref="WindowFactoryRegistrationSettings"/>.
        /// </param>
        /// <returns>
        ///     The current <see cref="IServiceCollection"/> so other actions
        ///     can be chained.
        /// </returns>
        public IServiceCollection RegisterWindowFactory(Action<WindowFactoryRegistrationSettings> registration) {
            var settings = ActionHelper.FromDelegate(registration);

            self.TryAdd(
                descriptors: CreateWindowServiceDescriptors(settings)
            );

            self.TryAddSingleton<IWindowFactory, WindowFactory>();

            return self;
        }
    }

    private static IEnumerable<ServiceDescriptor> CreateWindowServiceDescriptors(WindowFactoryRegistrationSettings settings) {
        var service = typeof(IWindow);
        var implementations = settings.UseAssemblyScan
            ? settings.ExecuteAssemblyScan(service)
            : settings.Windows;

        foreach (var implementation in implementations) {
            var lifetime = ServiceLifetimeAttribute.GetLifetime(implementation);
            var interfaces = implementation.GetInterfaces()
                                           .Where(service.IsAssignableFrom);

            foreach (var @interface in interfaces) {
                // Skip the main service interface
                if (@interface == service) { continue; }

                // Register the service with its extended interface
                yield return new ServiceDescriptor(@interface, implementation, lifetime);
            }

            // Register the service as concrete type
            yield return new ServiceDescriptor(implementation, implementation, lifetime);
        }
    }
}