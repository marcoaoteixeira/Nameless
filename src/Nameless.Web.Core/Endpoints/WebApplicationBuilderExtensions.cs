using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Nameless.Web.Endpoints;

/// <summary>
/// <see cref="WebApplicationBuilder"/> extension methods for registering endpoints.
/// </summary>
public static class WebApplicationBuilderExtensions {
    /// <summary>
    ///     Configures minimal endpoint services for the application,
    ///     including OpenAPI, versioning, and all implemented endpoints.
    /// </summary>
    /// <remarks>
    ///     This method sets up essential services required for minimal
    ///     endpoint functionality, including OpenAPI documentation, API
    ///     versioning, and core application services.
    /// 
    ///     Use the <paramref name="configure"/> parameter to customize
    ///     endpoint options, such as enabling or disabling specific features.
    /// </remarks>
    /// <param name="self">The current <see cref="WebApplicationBuilder"/>.</param>
    /// <param name="configure">
    ///     An optional delegate to configure <see cref="EndpointOptions"/> for
    ///     customizing endpoint behavior. If not provided, default options are
    ///     used.
    /// </param>
    /// <returns>
    ///     The current <see cref="IServiceCollection"/> so other actions can be chained.
    /// </returns>
    public static WebApplicationBuilder RegisterMinimalEndpoints(this WebApplicationBuilder self, Action<EndpointOptions>? configure = null) {
        var innerConfigure = configure ?? (_ => { });
        var options = new EndpointOptions();

        innerConfigure(options);

        self.Services
            .AddProblemDetails()
            .AddOpenApi(options.ConfigureOpenApi ?? (_ => { }))
            .AddApiVersioning(options.ConfigureApiVersioning ?? DefaultConfigureApiVersioningOptions)
            .AddApiExplorer(options.ConfigureApiExplorer ?? DefaultConfigureApiExplorerOptions);

        // Register all endpoints that implement IEndpoint interface
        var service = typeof(IEndpoint);
        var endpoints = options.Assemblies
                               .GetImplementations(service)
                               .Where(type => !type.IsGenericTypeDefinition);

        foreach (var endpoint in endpoints) {
            self.Services.AddScoped(service, endpoint);
        }

        return self;
    }

    private static void DefaultConfigureApiVersioningOptions(ApiVersioningOptions options) {
        // Add the headers "api-supported-versions" and "api-deprecated-versions"
        // This is better for discoverability
        options.ReportApiVersions = true;

        // AssumeDefaultVersionWhenUnspecified should only be enabled when supporting legacy services that did not previously
        // support API versioning. Forcing existing clients to specify an explicit API version for an
        // existing service introduces a breaking change. Conceptually, clients in this situation are
        // bound to some API version of a service, but they don't know what it is and never explicit request it.
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.DefaultApiVersion = new ApiVersion(1);

        // Defines how an API version is read from the current HTTP request
        options.ApiVersionReader = ApiVersionReader.Combine(
            new HeaderApiVersionReader(Constants.API_VERSION_HEADER_KEY),
            new UrlSegmentApiVersionReader());
    }

    private static void DefaultConfigureApiExplorerOptions(ApiExplorerOptions options) {
        // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
        // note: the specified format code will format the version as "'v'major[.minor][-status]"
        options.GroupNameFormat = Constants.API_GROUP_NAME_FORMAT;

        // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
        // can also be used to control the format of the API version in route templates
        options.SubstituteApiVersionInUrl = true;
    }
}