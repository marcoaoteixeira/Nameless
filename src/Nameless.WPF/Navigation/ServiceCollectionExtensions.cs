using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Nameless.Helpers;
using Nameless.WPF.DependencyInjection;
using Wpf.Ui;
using Wpf.Ui.Abstractions;
using Wpf.Ui.Abstractions.Controls;

namespace Nameless.WPF.Navigation;

/// <summary>
///     <see cref="IServiceCollection"/> extension methods.
/// </summary>
public static class ServiceCollectionExtensions {
    extension(IServiceCollection self) {
        public IServiceCollection RegisterNavigation(Action<NavigationRegistrationSettings> registration) {
            var settings = ActionHelper.FromDelegate(registration);

            self.RegisterNavigationService();
            self.RegisterNavigationWindow(settings);
            self.RegisterNavigationViewItemProvider(settings);
            self.RegisterNavigableViews(settings);

            return self;
        }

        private void RegisterNavigationService() {
            // Navigation services for WPF-UI
            self.TryAddSingleton<INavigationViewPageProvider, NavigationViewPageProvider>();
            self.TryAddSingleton<INavigationService, NavigationService>();
        }

        private void RegisterNavigationWindow(NavigationRegistrationSettings settings) {
            var service = typeof(INavigationWindow);
            var implementation = settings.UseAssemblyScan
                ? settings.ExecuteAssemblyScan(service).SingleOrDefault()
                : settings.NavigationWindow;

            if (implementation is null) {
                throw new InvalidOperationException($"Unable to locate implementation for type '{service.Name}'.");
            }

            self.TryAdd(implementation.CreateServiceDescriptor(service));
        }

        private void RegisterNavigationViewItemProvider(NavigationRegistrationSettings settings) {
            self.TryAddSingleton<INavigationViewItemProvider>(
                new AutoDiscoverableNavigationViewItemProvider(settings.Assemblies)
            );
        }

        private void RegisterNavigableViews(NavigationRegistrationSettings settings) {
            var service = typeof(INavigableView<>);
            var implementations = settings.UseAssemblyScan
                ? settings.ExecuteAssemblyScan(service)
                : settings.NavigationViews;

            foreach (var implementation in implementations) {
                var lifetime = ServiceLifetimeAttribute.GetLifetime(implementation);
                var interfaces = implementation.GetInterfaces()
                                               .Where(@interface => @interface.GenericTypeArguments.Length > 0 &&
                                                                    service.IsAssignableFromGeneric(@interface));
                foreach (var @interface in interfaces) {
                    // Register the navigable view's interface.
                    self.TryAdd(new ServiceDescriptor(@interface, implementation, lifetime));

                    // Register the navigable view viewmodel.
                    var viewModelType = @interface.GetGenericArguments().First();
                    self.TryAdd(new ServiceDescriptor(viewModelType, viewModelType, lifetime));
                }

                // Register the navigable view concrete type.
                self.TryAdd(new ServiceDescriptor(implementation, implementation, lifetime));
            }
        }
    }
}