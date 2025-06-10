using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;

namespace Nameless.Web.Endpoints;

/// <summary>
/// <see cref="IServiceCollection"/> extension methods for registering endpoints.
/// </summary>
public static class ServiceCollectionExtensions {
    public static IServiceCollection RegisterMinimalEndpoints(this IServiceCollection self, Action<EndpointOptions>? configure = null) {
        var innerConfigure = configure ?? (_ => { });
        var options = new EndpointOptions();

        innerConfigure(options);

        return self.RegisterOpenApiServices(options)
                   .RegisterVersioningServices(options)
                   .RegisterMainServices(options);
    }

    private static IServiceCollection RegisterOpenApiServices(this IServiceCollection self, EndpointOptions options) {
        return self.AddOpenApi(options.ConfigureOpenApi ?? (_ => { }));
    }

    private static IServiceCollection RegisterVersioningServices(this IServiceCollection self, EndpointOptions options) {
        self.AddApiVersioning(options.ConfigureApiVersioning ?? DefaultConfigureApiVersioningOptions)
            .AddApiExplorer(options.ConfigureApiExplorer ?? DefaultConfigureApiExplorerOptions);

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
        options.GroupNameFormat = "'v'VVV";

        // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
        // can also be used to control the format of the API version in route templates
        options.SubstituteApiVersionInUrl = true;
    }

    private static IServiceCollection RegisterMainServices(this IServiceCollection self, EndpointOptions options) {
        var serviceType = typeof(IEndpoint);
        var endpoints = options.Assemblies
                               .GetImplementations([serviceType])
                               .Where(type => !type.IsGenericTypeDefinition);

        foreach (var endpoint in endpoints) {
            self.AddScoped(serviceType, endpoint);
        }

        return self;
    }
}