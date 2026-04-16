using Microsoft.AspNetCore.Builder;
using Nameless.Web.OpenApi;

namespace Nameless.Web.Hosting.Configs;

public static class OpenApiConfig {
    extension(WebApplicationBuilder self) {
        /// <summary>
        ///     Configures OpenApi feature.
        /// </summary>
        /// <returns>
        ///     The current <see cref="WebApplicationBuilder"/> instance so
        ///     other actions can be chained.
        /// </returns>
        public WebApplicationBuilder ConfigureOpenApi(WebHostSettings settings) {
            if (settings.DisableOpenApi) { return self; }

            self.Services.RegisterOpenApi(settings.OpenApiRegistrationConfiguration ?? DefaultOpenApiConfig);

            return self;

            static void DefaultOpenApiConfig(OpenApiRegistration opts) {
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
        ///     The <see cref="WebHostSettings"/> instance.
        /// </param>
        /// <returns>
        ///     The current <see cref="WebApplication"/> so other actions
        ///     can be chained.
        /// </returns>
        public WebApplication UseOpenApi(WebHostSettings settings) {
            if (settings.DisableOpenApi) { return self; }

            self.MapOpenApi();

            return self;
        }
    }
}
