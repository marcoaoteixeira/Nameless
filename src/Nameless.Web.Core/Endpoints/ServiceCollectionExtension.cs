using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Nameless.Web.Endpoints;
public static class ServiceCollectionExtension {
    /// <summary>
    /// Registers all implementations of <see cref="EndpointBase"/>.
    /// </summary>
    /// <param name="self">The current <see cref="IServiceCollection"/> instance.</param>
    /// <param name="assemblies">The assemblies that will be mapped.</param>
    /// <returns>
    /// The current <see cref="IServiceCollection"/> instance so other actions can be chained.
    /// </returns>
    public static IServiceCollection AddMinimalEndpoints(this IServiceCollection self, Assembly[] assemblies) {
        Prevent.Argument.Null(self);
        Prevent.Argument.Null(assemblies);

        var endpointType = typeof(EndpointBase);
        var endpointImplementations = assemblies.SelectMany(assembly => assembly.SearchForImplementations<EndpointBase>());
        foreach (var endpointImplementation in endpointImplementations) {
            self.AddTransient(serviceType: endpointType,
                              implementationType: endpointImplementation);
        }
        return self;
    }
}
