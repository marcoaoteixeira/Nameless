// ReSharper disable SeparateLocalFunctionsWithJumpStatement

using Asp.Versioning;
using Asp.Versioning.Builder;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Nameless.Web.Endpoints;

/// <summary>
/// Extension methods for <see cref="IApplicationBuilder"/> to register endpoints.
/// </summary>
public static class ApplicationBuilderExtensions {
    /// <summary>
    /// Registers endpoints in the application options.
    /// </summary>
    /// <param name="self">The application options.</param>
    /// <returns>
    /// The current <see cref="IApplicationBuilder"/> instance so other actions can be chained.
    /// </returns>
    /// <remarks>
    /// It's necessary to call <c>UseRouting</c> before calling <c>UseMinimalEndpoints</c>.
    /// </remarks>
    public static IApplicationBuilder UseMinimalEndpoints(this IApplicationBuilder self) {
        self.UseEndpoints(builder => {
            using var scope = self.ApplicationServices.CreateScope();

            // resolve all our endpoints so we can configure them.
            var endpoints = scope.ServiceProvider.GetServices<IEndpoint>();

            // create all endpoint builders and configure them.
            var endpointBuilders = endpoints.Select(CreateEndpointBuilder)
                                            .ToArray();

            // create the version set object.
            var versionSet = builder.CreateVersionSet(endpointBuilders);

            // group all endpoint builders so we can configure them
            // by group name
            var endpointBuilderGroups = endpointBuilders.GroupBy(item => item.GetGroupName())
                                                        .ToArray();

            // finish the endpoint configuration
            foreach (var endpointBuilderGroup in endpointBuilderGroups) {
                builder.ConfigureEndpointBuilderGroup(endpointBuilderGroup, versionSet);
            }
        });

        return self;

        static EndpointBuilder CreateEndpointBuilder(IEndpoint endpoint) {
            var config = new EndpointBuilder(endpoint.GetType());
            endpoint.Configure(config);
            return config;
        }
    }

    private static ApiVersionSet CreateVersionSet(this IEndpointRouteBuilder self, IEnumerable<EndpointBuilder> endpointBuilders) {
        // extract all available versions from the groups.
        var availableVersions = endpointBuilders.Select(CreateVersionMetadata)
                                                .Distinct()
                                                .ToArray();

        // we need to add the versioning capability to the
        // builder, so calling NewVersionedApi() will do the trick.
        var versionSetBuilder = self.NewApiVersionSet();
        foreach (var availableVersion in availableVersions) {
            var apiVersion = new ApiVersion(availableVersion.Number);
            switch (availableVersion.Deprecated) {
                case true:
                    versionSetBuilder.HasDeprecatedApiVersion(apiVersion);
                    break;
                default:
                    versionSetBuilder.HasApiVersion(apiVersion);
                    break;
            }
        }

        return versionSetBuilder.ReportApiVersions().Build();

        static (int Number, bool Deprecated) CreateVersionMetadata(EndpointBuilder endpointBuilder) {
            return (
                endpointBuilder.Version,
                endpointBuilder.Stability == Stability.Deprecated
            );
        }
    }

    private static void ConfigureEndpointBuilderGroup(this IEndpointRouteBuilder self, IGrouping<string, EndpointBuilder> endpointBuilderGroup, ApiVersionSet versionSet) {
        var routeGroupBuilder = self.MapGroup(endpointBuilderGroup.Key)
                                    .WithApiVersionSet(versionSet);

        foreach (var endpointBuilder in endpointBuilderGroup) {
            endpointBuilder.Apply(routeGroupBuilder);
        }
    }
}