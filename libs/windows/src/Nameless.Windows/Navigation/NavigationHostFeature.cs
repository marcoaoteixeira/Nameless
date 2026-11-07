using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Nameless.Helpers;
using Nameless.Hosting;
using Wpf.Ui;
using Wpf.Ui.Abstractions;
using Wpf.Ui.Abstractions.Controls;

namespace Nameless.Windows.Navigation;

/// <summary>
///     Navigation Host Feature
/// </summary>
public class NavigationHostFeature : HostFeature<NavigationFeatureOptions> {
    /// <inheritdoc/>
    public override void Register(IHostApplicationBuilder builder, HostFeatureContext context) {
        var options = ActionHelper.FromDelegate(ConfigureFeature).WithAssemblies(context.Assemblies);
        var services = builder.Services;

        RegisterNavigationService(services);
        RegisterNavigationWindow(services, options);
        RegisterNavigationViewItemProvider(services, options);
        RegisterNavigableViews(services, options);
    }

    private static void RegisterNavigationService(IServiceCollection services) {
        // Navigation services for WPF-UI
        services.TryAddSingleton<INavigationViewPageProvider, NavigationViewPageProvider>();
        services.TryAddSingleton<INavigationService, NavigationService>();
    }

    private static void RegisterNavigationWindow(IServiceCollection services, NavigationFeatureOptions options) {
        if (options.NavigationWindow is null) {
            throw new InvalidOperationException(
                $"Unable to locate implementation for type '{nameof(INavigationWindow)}'."
            );
        }

        services.TryAdd(
            ServiceDescriptor.Transient(
                typeof(INavigationWindow),
                options.NavigationWindow
            )
        );
    }

    private static void RegisterNavigationViewItemProvider(IServiceCollection services, NavigationFeatureOptions options) {
        services.TryAddSingleton<INavigationViewItemProvider>(
            new AutoDiscoverableNavigationViewItemProvider(options.Assemblies)
        );
    }

    private static void RegisterNavigableViews(IServiceCollection services, NavigationFeatureOptions options) {
        var service = typeof(INavigableView<>);

        foreach (var implementation in options.NavigationViews) {
            var interfaces = implementation.GetInterfaces()
                                           .Where(@interface => @interface.GenericTypeArguments.Length > 0 &&
                                                                service.IsAssignableFromGeneric(@interface));
            foreach (var @interface in interfaces) {
                // Register the navigable view's interface.
                services.TryAdd(ServiceDescriptor.Transient(@interface, implementation));

                // Register the navigable view viewmodel.
                var viewModelType = @interface.GetGenericArguments().First();
                services.TryAdd(ServiceDescriptor.Transient(viewModelType, viewModelType));
            }

            // Register the navigable view concrete type.
            services.TryAdd(ServiceDescriptor.Transient(implementation, implementation));
        }
    }
}
