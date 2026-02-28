using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Nameless.Registration;

namespace Nameless.Web.MinimalEndpoints;

/// <summary>
///     Represents options for configuring endpoints.
/// </summary>
public class MinimalEndpointsRegistrationSettings : AssemblyScanAware<MinimalEndpointsRegistrationSettings> {
    private const string API_GROUP_NAME_FORMAT = "'v'VVV";
    private const string API_VERSION_HEADER_KEY = "api-version";

    private readonly HashSet<Type> _endpoints = [];

    public IReadOnlyCollection<Type> Endpoints => UseAssemblyScan
        ? DiscoverImplementationsFor<IEndpoint>(includeGenericDefinition: false)
        : _endpoints;

    /// <summary>
    ///     Gets or sets the action to configure API versioning options.
    /// </summary>
    public Action<ApiVersioningOptions> ConfigureApiVersioning { get; set; } = DefaultConfigureApiVersioningOptions;

    /// <summary>
    ///     Gets or sets the action to configure API explorer options.
    /// </summary>
    public Action<ApiExplorerOptions> ConfigureApiExplorer { get; set; } = DefaultConfigureApiExplorerOptions;

    public MinimalEndpointsRegistrationSettings RegisterEndpoint<TEndpoint>()
        where TEndpoint : IEndpoint {
        return RegisterEndpoint(typeof(TEndpoint));
    }

    public MinimalEndpointsRegistrationSettings RegisterEndpoint(Type type) {
        Throws.When.IsNotAssignableFrom(type, typeof(IEndpoint));
        Throws.When.IsOpenGenericType(type);
        Throws.When.IsNonConcreteType(type);

        _endpoints.Add(type);

        return this;
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