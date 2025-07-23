// ReSharper disable SeparateLocalFunctionsWithJumpStatement

using Asp.Versioning;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

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
            using var scope = builder.ServiceProvider.CreateScope();

            // resolve all our endpoints so we can configure them.
            var endpoints = scope.ServiceProvider.GetServices<IEndpoint>();

            // create all endpoint builders and configure them.
            var endpointDescriptors = endpoints.Select(CreateEndpointDescriptor)
                                               .ToArray();

            // create the root group
            builder.MapGroup(ROOT_GROUP_ROUTE_PREFIX)
                   // define the version set for all endpoints.
                   .WithVersionSetFrom(endpointDescriptors)
                   // map all endpoints to the root group
                   .WithEndpoints(endpointDescriptors);
        });

        return self;

        static EndpointDescriptor CreateEndpointDescriptor(IEndpoint endpoint) {
            var config = new EndpointDescriptor(endpoint.GetType());
            endpoint.Configure(config);
            return config;
        }
    }

    private static RouteGroupBuilder WithVersionSetFrom(this RouteGroupBuilder self, IEnumerable<EndpointDescriptor> endpointDescriptors) {
        // extract all available versions from the groups.
        var availableVersions = endpointDescriptors.Select(endpoint => endpoint.Version)
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

    private static void WithEndpoints(this RouteGroupBuilder self, IEnumerable<EndpointDescriptor> endpointDescriptors) {
        // groups all endpoint descriptors by group name
        var groups = endpointDescriptors.GroupBy(item => item.GroupName);

        foreach (var group in groups) {
            // with the group name, we create a new group builder for the
            // similar endpoints.
            var endpointGroup = self.MapGroup(group.Key);

            // finally, we apply all endpoint descriptors to the group
            // builder.
            foreach (var endpointDescriptor in group) {
                endpointDescriptor.Apply(endpointGroup);
            }
        }
    }
}