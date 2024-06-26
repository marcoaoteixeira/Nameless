﻿using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Nameless.Web.Options {
    /// <summary>
    /// Configures the Swagger generation options.
    /// </summary>
    /// <remarks>This allows API versioning to define a Swagger document per API version after the
    /// <see cref="IApiVersionDescriptionProvider"/> service has been resolved from the service container.</remarks>
    public sealed class SwaggerGenConfigureOptions : IConfigureOptions<SwaggerGenOptions> {
        #region Private Read-Only Fields

        private readonly IApiVersionDescriptionProvider _provider;
        private readonly IHostEnvironment _hostEnvironment;
        private readonly SwaggerPageOptions _options;

        #endregion

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigureSwaggerOptions"/> class.
        /// </summary>
        /// <param name="provider">The <see cref="IApiVersionDescriptionProvider">provider</see> used to generate Swagger documents.</param>
        /// <param name="hostEnvironment">The <see cref="IHostEnvironment"/>hostEnvironment.</param>
        /// <param name="options">The <see cref="SwaggerPageOptions"/>applicationOptions.</param>
        public SwaggerGenConfigureOptions(IApiVersionDescriptionProvider provider, IHostEnvironment hostEnvironment, SwaggerPageOptions? options = null) {
            _provider = Guard.Against.Null(provider, nameof(provider));
            _hostEnvironment = Guard.Against.Null(hostEnvironment, nameof(hostEnvironment));
            _options = options ?? SwaggerPageOptions.Default;
        }

        #endregion

        #region Private Static Methods

        private static OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description, IHostEnvironment hostEnvironment, SwaggerPageOptions settings)
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

        #endregion

        #region IConfigureOptions<SwaggerGenOptions>

        /// <inheritdoc />
        public void Configure(SwaggerGenOptions options) {
            // add a swagger document for each discovered API version
            // note: you might choose to skip or document deprecated API versions differently
            foreach (var description in _provider.ApiVersionDescriptions) {
                options.SwaggerDoc(name: description.GroupName,
                                   info: CreateInfoForApiVersion(description: description,
                                                                 hostEnvironment: _hostEnvironment,
                                                                 settings: _options));
            }
        }

        #endregion
    }
}
