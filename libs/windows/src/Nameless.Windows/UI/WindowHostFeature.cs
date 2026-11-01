using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Nameless.Helpers;
using Nameless.Hosting;

namespace Nameless.Windows.UI;

/// <summary>
///     Window Host Feature
/// </summary>
public class WindowHostFeature : HostFeature<WindowFeatureOptions> {
    /// <inheritdoc/>
    public override void Register(IHostApplicationBuilder builder, HostFeatureContext context) {
        var options = ActionHelper.FromDelegate(ConfigureFeature)
                                  .WithAssemblies(context.Assemblies);

        builder.Services.TryAdd(
            descriptors: CreateServiceDescriptorsForWindow(options)
        );

        builder.Services.TryAddSingleton<IWindowFactory, WindowFactory>();
    }

    private static IEnumerable<ServiceDescriptor> CreateServiceDescriptorsForWindow(WindowFeatureOptions settings) {
        var service = typeof(IWindow);

        foreach (var implementation in settings.Windows) {
            var interfaces = implementation.GetInterfaces()
                                           .Where(service.IsAssignableFrom);

            foreach (var @interface in interfaces) {
                // Skip the main service interface
                if (@interface == service) { continue; }

                // Register the service with its extended interface
                yield return ServiceDescriptor.Transient(@interface, implementation);
            }

            // Register the service as concrete type
            yield return ServiceDescriptor.Transient(implementation, implementation);
        }
    }
}