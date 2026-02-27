using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Nameless.Helpers;

namespace Nameless.Web.OpenApi;

/// <summary>
///     <see cref="WebApplicationBuilder"/> extension methods
/// </summary>
public static class WebApplicationBuilderExtensions {
    /// <param name="self">
    ///     The current <see cref="WebApplicationBuilder"/> instance.
    /// </param>
    extension(WebApplicationBuilder self) {
        /// <summary>
        ///     Registers OpenAPI services for the application using the
        ///     specified registration settings.
        /// </summary>
        /// <remarks>
        ///     If no document options are specified in the registration
        ///     settings, the default OpenAPI services are added.
        ///     Otherwise, each provided document option is registered
        ///     individually.
        /// </remarks>
        /// <param name="registration">
        ///     An action that configures the OpenAPI registration settings,
        ///     including document options for the API. Cannot be null.
        /// </param>
        /// <returns>
        ///     The current instance of the <see cref="WebApplicationBuilder"/>
        ///     so other actions can be chained.
        /// </returns>
        public WebApplicationBuilder RegisterOpenApi(Action<OpenApiRegistrationSettings> registration) {
            var settings = ActionHelper.FromDelegate(registration);

            if (settings.DocumentOptions.Count == 0) {
                self.Services.AddOpenApi();

                return self;
            }

            foreach (var document in settings.DocumentOptions) {
                self.Services.AddOpenApi(document.Key, document.Value);
            }

            return self;
        }
    }
}