using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Nameless.Web.Options;

/// <summary>
/// Configures Swagger generation options.
/// </summary>
public sealed class SwaggerGenConfigureOptions : IConfigureOptions<SwaggerGenOptions> {
    private readonly IHostEnvironment _hostEnvironment;
    private readonly SwaggerPageOptions _options;
    private readonly IApiVersionDescriptionProvider _provider;

    /// <summary>
    /// Initializes a new instance of the <see cref="SwaggerGenConfigureOptions" /> class.
    /// </summary>
    /// <param name="hostEnvironment">The host environment.</param>
    /// <param name="options">The Swagger page options.</param>
    public SwaggerGenConfigureOptions(IHostEnvironment hostEnvironment, IOptions<SwaggerPageOptions> options)
        : this(NullApiVersionDescriptionProvider.Instance, hostEnvironment, options) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="SwaggerGenConfigureOptions" /> class.
    /// </summary>
    /// <param name="apiVersionDescriptionProvider">The API version description provider.</param>
    /// <param name="hostEnvironment">The host environment.</param>
    /// <param name="options">The Swagger page options.</param>
    public SwaggerGenConfigureOptions(IApiVersionDescriptionProvider apiVersionDescriptionProvider, IHostEnvironment hostEnvironment, IOptions<SwaggerPageOptions> options) {
        _provider = Prevent.Argument.Null(apiVersionDescriptionProvider);
        _hostEnvironment = Prevent.Argument.Null(hostEnvironment);
        _options = Prevent.Argument.Null(options).Value;
    }

    /// <inheritdoc />
    public void Configure(SwaggerGenOptions options) {
        // add a swagger document for each discovered API version
        // note: you might choose to skip or document deprecated API versions differently
        foreach (var description in _provider.ApiVersionDescriptions) {
            options.SwaggerDoc(description.GroupName,
                               CreateInfoForApiVersion(description,
                                                       _hostEnvironment,
                                                       _options));
        }
    }

    private static OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description,
                                                       IHostEnvironment hostEnvironment,
                                                       SwaggerPageOptions settings)
        => new() {
            Title = hostEnvironment.ApplicationName,
            Version = description.ApiVersion.ToString(),
            Description = $"{(description.IsDeprecated ? "[DEPRECATED] " : string.Empty)}{settings.Description}",
            Contact = new OpenApiContact {
                Name = settings.Contact.Name,
                Email = settings.Contact.Email,
                Url = new Uri(settings.Contact.Url)
            },
            License = new OpenApiLicense {
                Name = settings.License.Name,
                Url = new Uri(settings.License.Url)
            }
        };
}