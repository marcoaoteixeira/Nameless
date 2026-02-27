using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Nameless.Web.OpenApi;
using Scalar.AspNetCore;

namespace Nameless.Microservices.StartUp;

public static partial class WebAppExtensions {
    extension(WebApplicationBuilder self) {
        /// <summary>
        ///     Configures OpenApi feature.
        /// </summary>
        /// <returns>
        ///     The current <see cref="WebApplicationBuilder"/> instance so
        ///     other actions can be chained.
        /// </returns>
        public WebApplicationBuilder OpenApiConfig(WebAppSettings settings) {
            if (settings.DisableOpenApi) { return self; }

            self.RegisterOpenApi(settings.ConfigureOpenApi ?? DefaultConfiguration);

            return self;

            static void DefaultConfiguration(OpenApiRegistrationSettings opts) {
                opts.RegisterOpenApiDocument("v1", options => {
                    options.AddDocumentTransformer<BearerSecuritySchemeDocumentTransformer>();
                    options.AddOperationTransformer<DeprecateOpenApiOperationTransformer>();
                });
            }
        }
    }

    /// <param name="self">
    ///     The current <see cref="WebApplication"/> instance.
    /// </param>
    extension(WebApplication self) {
        /// <summary>
        ///     Uses the OpenAPI service.
        /// </summary>
        /// <param name="settings">
        ///     The <see cref="WebAppSettings"/> instance.
        /// </param>
        /// <returns>
        ///     The current <see cref="WebApplication"/> so other actions
        ///     can be chained.
        /// </returns>
        public WebApplication UseOpenApi(WebAppSettings settings) {
            if (settings.DisableOpenApi) { return self; }

            self.MapOpenApi();

            // Do not expose Scalar on PROD environment
            if (self.Environment.IsDevelopment()) {
                self.MapScalarApiReference(scalar =>
                    scalar
                        .WithTitle(self.Environment.ApplicationName)
                        .WithTheme(ScalarTheme.BluePlanet)
                        .WithDefaultHttpClient(ScalarTarget.Shell, ScalarClient.Curl)
                );
            }

            return self;
        }
    }
}
