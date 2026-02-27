using Microsoft.AspNetCore.Builder;
using Nameless.Validation.FluentValidation;

namespace Nameless.Microservices.StartUp;

public static partial class WebAppExtensions {
    extension(WebApplicationBuilder self) {
        /// <summary>
        ///     Configures Validation feature.
        /// </summary>
        /// <returns>
        ///     The current <see cref="WebApplicationBuilder"/> instance so
        ///     other actions can be chained.
        /// </returns>
        public WebApplicationBuilder ValidationConfig(WebAppSettings settings) {
            if (settings.DisableValidation) { return self; }

            self.Services.RegisterValidation(settings.ConfigureValidation ?? DefaultConfiguration);

            return self;

            void DefaultConfiguration(ValidationRegistrationSettings opts) {
                opts.IncludeAssemblies(settings.Assemblies);
            }
        }
    }
}