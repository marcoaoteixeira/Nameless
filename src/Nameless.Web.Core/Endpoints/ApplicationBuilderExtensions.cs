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
    public const string ROOT_ROUTE_PREFIX = "api/v{v:apiVersion}";

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
            using var scope = self.ApplicationServices.CreateScope();

            // resolve all our endpoints so we can configure them.
            var endpoints = scope.ServiceProvider.GetServices<IEndpoint>();

            // create all endpoint builders and configure them.
            var endpointBuilders = endpoints.Select(CreateEndpointDescriptor)
                                            .ToArray();

            // create the group for the API version
            var rootMapGroup = builder.MapGroup(ROOT_ROUTE_PREFIX);

            // create the version set object.
            var versionSet = builder.CreateVersionSet(endpointBuilders);

            // group all endpoint builders so we can configure them
            // by group name
            var endpointBuilderGroups = endpointBuilders.GroupBy(item => item.GroupName)
                                                        .ToArray();

            // finish the endpoint configuration
            foreach (var endpointBuilderGroup in endpointBuilderGroups) {
                builder.ConfigureEndpointBuilderGroup(endpointBuilderGroup, versionSet);
            }
        });

        return self;

        static EndpointDescriptor CreateEndpointDescriptor(IEndpoint endpoint) {
            var config = new EndpointDescriptor(endpoint.GetType());
            endpoint.Configure(config);
            return config;
        }
    }

    private static ApiVersionSet CreateVersionSet(this IEndpointRouteBuilder self, IEnumerable<EndpointDescriptor> endpointDescriptors) {
        // extract all available versions from the groups.
        var availableVersions = endpointDescriptors.Select(CreateVersionMetadata)
                                                   .Distinct()
                                                   .ToArray();

        // we need to add the versioning capability to the
        // descriptor, so calling NewVersionedApi() will do the trick.
        var versionSetBuilder = self.NewApiVersionSet();
        foreach (var (number, deprecated) in availableVersions) {
            var apiVersion = new ApiVersion(number);
            switch (deprecated) {
                case true:
                    versionSetBuilder.HasDeprecatedApiVersion(apiVersion);
                    break;
                default:
                    versionSetBuilder.HasApiVersion(apiVersion);
                    break;
            }
        }

        return versionSetBuilder.ReportApiVersions().Build();

        static (int Number, bool Deprecated) CreateVersionMetadata(EndpointDescriptor endpointDescriptor) {
            return (
                endpointDescriptor.Version,
                endpointDescriptor.Stability == Stability.Deprecated
            );
        }
    }

    private static void ConfigureEndpointBuilderGroup(this IEndpointRouteBuilder self, IGrouping<string, EndpointDescriptor> endpointDescriptorGroup, ApiVersionSet versionSet) {
        var routeGroupBuilder = self.MapGroup(endpointDescriptorGroup.Key)
                                    .WithApiVersionSet(versionSet);

        foreach (var endpointDescriptor in endpointDescriptorGroup) {
            endpointDescriptor.Apply(routeGroupBuilder);
        }
    }
}