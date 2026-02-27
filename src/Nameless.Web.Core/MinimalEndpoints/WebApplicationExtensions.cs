// ReSharper disable SuspiciousTypeConversion.Global

using Asp.Versioning;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Nameless.Web.MinimalEndpoints.Definitions;
using Nameless.Web.MinimalEndpoints.Infrastructure;

namespace Nameless.Web.MinimalEndpoints;

/// <summary>
///     Extension methods for <see cref="IApplicationBuilder"/>.
/// </summary>
public static class WebApplicationExtensions {
    private const string ROOT_GROUP_ROUTE_PREFIX = "api/v{version:apiVersion}";

    /// <param name="self">
    ///     The current <see cref="WebApplication"/>.
    /// </param>
    extension(WebApplication self) {
        /// <summary>
        ///     Registers endpoints in the application options.
        /// </summary>
        /// <returns>
        ///     The current <see cref="WebApplication"/> instance so other
        ///     actions can be chained.
        /// </returns>
        /// <remarks>
        ///     It's necessary to call <c>UseRouting</c> before
        ///     calling <see cref="WebApplicationExtensions.UseMinimalEndpoints"/>.
        ///     If you are using authorization, you also need to call
        ///     <c>UseAuthorization</c> before this method.
        /// </remarks>
        public WebApplication UseMinimalEndpoints() {
            // Why are we suppressing diagnostic warning ASP0014? See: https://learn.microsoft.com/pt-br/aspnet/core/diagnostics/asp0014
            // Because we are mapping the endpoints dynamically, based on the
            // endpoint descriptor we create on-the-fly.
#pragma warning disable ASP0014
            self.UseEndpoints(builder => {
                // Creates all endpoints descriptors.
                var endpointDescriptors = builder.ServiceProvider
                                                 .CreateEndpointDescriptors();

                // Create the root group
                builder.MapGroup(ROOT_GROUP_ROUTE_PREFIX)
                       // Define the version set for all endpoints.
                       .WithVersionSetFrom(endpointDescriptors)
                       // Map all endpoints to the root group
                       .WithEndpoints(endpointDescriptors);
            });
#pragma warning restore ASP0014

            return self;
        }
    }

    extension(RouteGroupBuilder self) {
        private RouteGroupBuilder WithVersionSetFrom(IEnumerable<IEndpointDescriptor> endpointDefinitions) {
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

        private void WithEndpoints(IEnumerable<IEndpointDescriptor> endpointDefinitions) {
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
    }

    extension(IServiceProvider self) {
        private IEndpointDescriptor[] CreateEndpointDescriptors() {
            // We need this "ServiceFactory" because we need to create
            // instances of the endpoints on-the-fly. We cannot register
            // them into the service collection because they might need
            // services that are registered as transient or scoped, and
            // we need to create them dynamically based on the request
            // context.
            var serviceResolver = self.GetRequiredService<IServiceFactory>();

            // Gets all the endpoint types from the service collection.
            var endpointTypeCollection = self.GetRequiredService<EndpointTypeCollection>();

            // Creates the endpoints descriptors.
            var result = new List<IEndpointDescriptor>();

            foreach (var endpointType in endpointTypeCollection) {
                if (serviceResolver.Create(endpointType) is not IEndpoint endpoint) {
                    throw new InvalidOperationException($"Unable to create instance of '{endpointType.FullName}'.");
                }

                result.Add(endpoint.Describe());

                if (endpoint is IDisposable disposable) {
                    disposable.Dispose();
                }
            }

            return [.. result];
        }
    }
}