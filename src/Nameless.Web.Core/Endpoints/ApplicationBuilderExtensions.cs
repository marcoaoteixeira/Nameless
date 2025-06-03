using Asp.Versioning;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Scalar.AspNetCore;

namespace Nameless.Web.Endpoints;

/// <summary>
/// Extension methods for <see cref="IApplicationBuilder"/> to register endpoints.
/// </summary>
public static class ApplicationBuilderExtensions {
    /// <summary>
    /// Registers endpoints in the application options.
    /// </summary>
    /// <param name="self">The application options.</param>
    /// <param name="configureScalar">Configure Scalar Open API.</param>
    /// <returns>
    /// The current <see cref="IApplicationBuilder"/> instance so other actions can be chained.
    /// </returns>
    /// <remarks>
    /// We use Scalar to generate the Open API specification for our endpoints.
    /// See more at: <a href="https://scalar.com/">Scalar</a>.
    /// <br /><br />
    /// It's necessary to call <c>UseRouting</c> before calling <c>UseMinimalEndpoints</c>.
    /// Also, if you are using Open API, you need to call <c>MapOpenApi</c> before calling this
    /// method either. To ensure that the Open API specification is generated correctly.
    /// </remarks>
    public static IApplicationBuilder UseMinimalEndpoints(this IApplicationBuilder self, Action<ScalarOptions>? configureScalar = null) {
        self.UseEndpoints(builder => {
            using var scope = self.ApplicationServices.CreateScope();

            var endpoints = scope.ServiceProvider.GetServices<IEndpoint>();
            var groups = endpoints.Select(CreateEndpointConfiguration)
                                  .GroupBy(tuple => tuple.Configuration.VersionSet);

            foreach (var group in groups) {
                var versionSetBuilder = builder.NewApiVersionSet(group.Key);
                var versionDefinitions = group.Select(item => (
                    item.Configuration.Version,
                    item.Configuration.Deprecated
                ));

                foreach (var versionDefinition in versionDefinitions.Distinct()) {
                    var apiVersion = new ApiVersion(versionDefinition.Version);

                    versionSetBuilder = versionDefinition.Deprecated
                        ? versionSetBuilder.HasDeprecatedApiVersion(apiVersion)
                        : versionSetBuilder.HasApiVersion(apiVersion);
                }

                var versionSet = versionSetBuilder.ReportApiVersions()
                                                  .Build();

                foreach (var item in group) {
                    item.Configuration.Apply(builder, item.Endpoint, versionSet);
                }
            }

            // Configure Scalar last.
            builder.MapScalarApiReference(configureScalar ?? (_ => { }));
        });

        return self;

        static (EndpointBuilder Configuration, IEndpoint Endpoint) CreateEndpointConfiguration(IEndpoint endpoint) {
            var config = new EndpointBuilder();
            endpoint.Configure(config);
            return (Configuration: config, Endpoint: endpoint);
        }
    }
}