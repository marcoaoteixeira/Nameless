using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Nameless.Web.Endpoints.Infrastructure;

namespace Nameless.Web.Endpoints;

/// <summary>
///     Host application builder extension methods.
/// </summary>
public static class HostApplicationBuilderExtensions {
    private const string API_GROUP_NAME_FORMAT = "'v'VVV";
    private const string API_VERSION_HEADER_KEY = "api-version";

    /// <summary>
    ///     Configures minimal endpoint services for the application,
    ///     including OpenAPI, versioning, and all implemented endpoints.
    /// </summary>
    /// <typeparam name="THostApplicationBuilder">
    ///     Type that implements <see cref="IHostApplicationBuilder"/>.
    /// </typeparam>
    /// <remarks>
    ///     This method sets up essential services required for minimal
    ///     endpoint functionality, including OpenAPI documentation, API
    ///     versioning, and core application services.
    /// 
    ///     Use the <paramref name="configure"/> parameter to customize
    ///     endpoint options, such as enabling or disabling specific features.
    /// </remarks>
    /// <param name="self">
    ///     The current <typeparamref name="THostApplicationBuilder"/>.
    /// </param>
    /// <param name="configure">
    ///     An optional delegate to configure <see cref="EndpointOptions"/> for
    ///     customizing endpoint behavior. If not provided, default options are
    ///     used.
    /// </param>
    /// <returns>
    ///     The current <typeparamref name="THostApplicationBuilder"/>
    ///     so other actions can be chained.
    /// </returns>
    public static THostApplicationBuilder RegisterMinimalEndpoints<THostApplicationBuilder>(
        this THostApplicationBuilder self, Action<EndpointOptions>? configure = null)
        where THostApplicationBuilder : IHostApplicationBuilder {
        var innerConfigure = configure ?? (_ => { });
        var options = new EndpointOptions();

        innerConfigure(options);

        self.Services
            .AddOpenApi(options.ConfigureOpenApi)
            .AddApiVersioning(options.ConfigureApiVersioning ?? DefaultConfigureApiVersioningOptions)
            .AddApiExplorer(options.ConfigureApiExplorer ?? DefaultConfigureApiExplorerOptions);

        var service = typeof(IEndpoint);
        var endpoints = options.Assemblies
                               .GetImplementations(service)
                               .Where(type => !type.IsGenericTypeDefinition);

        self.Services.TryAddSingleton(new EndpointTypeCollection(endpoints));
        self.Services.TryAddSingleton<IServiceFactory, ServiceFactory>();
        self.Services.TryAddSingleton<IEndpointFactory, EndpointFactory>();

        return self;
    }

    extension(IServiceCollection self) {
        private IServiceCollection AddOpenApi(Func<IEnumerable<OpenApiDocumentOptions>>? configure) {
            var descriptors = (configure?.Invoke() ?? []).ToArray();

            if (descriptors.Length == 0) {
                return self.AddOpenApi();
            }

            foreach (var descriptor in descriptors) {
                self.AddOpenApi(descriptor.DocumentName, descriptor.Options ?? (_ => { }));
            }

            return self;
        }
    }

    private static void DefaultConfigureApiVersioningOptions(ApiVersioningOptions options) {
        // Add the headers "api-supported-versions" and "api-deprecated-versions"
        // This is better for discoverability
        options.ReportApiVersions = true;

        // AssumeDefaultVersionWhenUnspecified should only be enabled when
        // supporting legacy services that did not previously support API
        // versioning. Forcing existing clients to specify an explicit API
        // version for an existing service introduces a breaking change.
        // Conceptually, clients in this situation are bound to some API
        // version of a service, but they don't know what it is and never
        // explicit request it.
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.DefaultApiVersion = new ApiVersion(majorVersion: 1);

        // Defines how an API version is read from the current HTTP request
        options.ApiVersionReader = ApiVersionReader.Combine(
            new HeaderApiVersionReader(API_VERSION_HEADER_KEY),
            new UrlSegmentApiVersionReader());
    }

    private static void DefaultConfigureApiExplorerOptions(ApiExplorerOptions options) {
        // add the versioned api explorer, which also adds
        // IApiVersionDescriptionProvider service
        // note: the specified format code will format the version
        // as "'v'major[.minor][-status]"
        options.GroupNameFormat = API_GROUP_NAME_FORMAT;

        // note: this option is only necessary when versioning by url segment.
        // The SubstitutionFormat can also be used to control the format of
        // the API version in route templates.
        options.SubstituteApiVersionInUrl = true;
    }
}