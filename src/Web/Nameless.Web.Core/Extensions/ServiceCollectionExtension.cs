using System.Reflection;
using Asp.Versioning;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Nameless.Web.Endpoints;
using Nameless.Web.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Nameless.Web;

public static class ServiceCollectionExtension {
    /// <summary>
    /// Adds Swashbuckle Swagger API discovery services. Also adds endpoints API explorer.
    /// </summary>
    /// <param name="self">The current <see cref="IServiceCollection" /> instance.</param>
    /// <param name="configure">The Swagger generator configuration action.</param>
    /// <returns>
    /// The current <see cref="IServiceCollection" /> instance so other actions can be chained.
    /// </returns>
    public static IServiceCollection AddSwagger(this IServiceCollection self, Action<SwaggerGenOptions>? configure = null)
        => Prevent.Argument
                  .Null(self)
                  .AddEndpointsApiExplorer()
                  .AddSwaggerGen(configure)
                  .AddTransient<IConfigureOptions<SwaggerGenOptions>, SwaggerGenConfigureOptions>();

    /// <summary>
    ///     Registers all implementations of <see cref="MinimalEndpointBase" />.
    /// </summary>
    /// <param name="self">The current <see cref="IServiceCollection" /> instance.</param>
    /// <param name="assemblies">The assemblies that will be mapped.</param>
    /// <returns>
    ///     The current <see cref="IServiceCollection" /> instance so other actions can be chained.
    /// </returns>
    public static IServiceCollection AddMinimalEndpoints(this IServiceCollection self, Assembly[] assemblies) {
        Prevent.Argument.Null(self);
        Prevent.Argument.Null(assemblies);

        var endpointType = typeof(MinimalEndpointBase);
        var endpointImplementations =
            assemblies.SelectMany(assembly => assembly.SearchForImplementations<MinimalEndpointBase>());

        foreach (var endpointImplementation in endpointImplementations) {
            self.AddScoped(endpointType, endpointImplementation);
        }

        return self;
    }

    /// <summary>
    ///     Adds API versioning services.
    /// </summary>
    /// <param name="self">The current <see cref="IServiceCollection" /> instance.</param>
    /// <returns>
    ///     The current <see cref="IServiceCollection" /> instance so other actions can be chained.
    /// </returns>
    public static IServiceCollection AddApiVersioningDefault(this IServiceCollection self) {
        self
            .AddApiVersioning(options => {
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
                options.ApiVersionReader = ApiVersionReader.Combine(new HeaderApiVersionReader(Constants.API_VERSION_HEADER_KEY),
                                                                    new UrlSegmentApiVersionReader());
            })
            .AddApiExplorer(opts => {
                // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
                // note: the specified format code will format the version as "'v'major[.minor][-status]"
                opts.GroupNameFormat = "'v'VVV";

                // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
                // can also be used to control the format of the API version in route templates
                opts.SubstituteApiVersionInUrl = true;
            });

        return self;
    }
}