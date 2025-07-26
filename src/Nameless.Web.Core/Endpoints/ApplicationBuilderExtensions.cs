// ReSharper disable SeparateLocalFunctionsWithJumpStatement

using Asp.Versioning;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Nameless.Web.Endpoints.Definitions;
using Nameless.Web.Endpoints.Infrastructure;

namespace Nameless.Web.Endpoints;

/// <summary>
/// Extension methods for <see cref="IApplicationBuilder"/> to register endpoints.
/// </summary>
public static class ApplicationBuilderExtensions {
    private const string ROOT_GROUP_ROUTE_PREFIX = "api/v{version:apiVersion}";

    /// <summary>
    ///     Registers endpoints in the application options.
    /// </summary>
    /// <typeparam name="TApplicationBuilder">Type of the application builder.</typeparam>
    /// <param name="self">The application options.</param>
    /// <returns>
    ///     The current <typeparamref name="TApplicationBuilder"/> instance so other actions can be chained.
    /// </returns>
    /// <remarks>
    ///     It's necessary to call <c>UseRouting</c> before calling <c>UseMinimalEndpoints</c>.
    ///     If you are using authorization, you also need to call <c>UseAuthorization</c> before this method.
    /// </remarks>
    public static TApplicationBuilder UseMinimalEndpoints<TApplicationBuilder>(this TApplicationBuilder self)
        where TApplicationBuilder : IApplicationBuilder {
        self.UseEndpoints(builder => {
            // Creates all endpoints descriptors.
            var endpointDescriptors = builder.ServiceProvider
                                             .CreateEndpointDescriptors();

            // create the root group
            builder.MapGroup(ROOT_GROUP_ROUTE_PREFIX)
                   // define the version set for all endpoints.
                   .WithVersionSetFrom(endpointDescriptors)
                   // map all endpoints to the root group
                   .WithEndpoints(endpointDescriptors);
        });

        return self;
    }

    private static RouteGroupBuilder WithVersionSetFrom(this RouteGroupBuilder self, IEnumerable<IEndpointDescriptor> endpointDefinitions) {
        // extract all available versions from the groups.
        var availableVersions = endpointDefinitions.Select(endpoint => endpoint.Version.Number)
                                                   .Distinct()
                                                   .ToArray();

        // creates the version set builder.
        var versionSetBuilder = self.NewApiVersionSet();

        foreach (var version in availableVersions) {
            versionSetBuilder.HasApiVersion(new ApiVersion(version));
        }

        // creates the version set.
        var versionSet = versionSetBuilder.ReportApiVersions().Build();

        // set the version set to the endpoint route builder.
        self.WithApiVersionSet(versionSet);

        return self;
    }

    private static void WithEndpoints(this RouteGroupBuilder self, IEnumerable<IEndpointDescriptor> endpointDefinitions) {
        // groups all endpoint descriptors by group name
        var groups = endpointDefinitions.GroupBy(item => item.GroupName);

        foreach (var group in groups) {
            // with the group name, we create a new group builder for the
            // similar endpoints.
            var routeGroupBuilder = self.MapGroup(group.Key);

            // finally, we apply all endpoint descriptors to the group
            // builder.
            foreach (var descriptor in group) {
                routeGroupBuilder.MapEndpoint(descriptor);
            }
        }
    }

    private static IEndpointDescriptor[] CreateEndpointDescriptors(this IServiceProvider self) {
        // We need this "ServiceResolver" to be available
        var serviceResolver = self.GetRequiredService<IServiceResolver>();

        // Gets all the endpoint types from the service collection.
        var endpointTypeCollection = self.GetRequiredService<EndpointTypeCollection>();

        // Creates the endpoints descriptors.
        var endpointDescriptors = new List<IEndpointDescriptor>();

        foreach (var endpointType in endpointTypeCollection) {
            if (serviceResolver.CreateInstance(endpointType) is not IEndpoint instance) {
                throw new InvalidOperationException($"Unable to create instance of '{endpointType.FullName}'.");
            }

            endpointDescriptors.Add(instance.Describe());
        }

        return [.. endpointDescriptors];
    }
}