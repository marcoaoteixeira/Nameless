using System.Reflection;
using Nameless.Validation.FluentValidation;

namespace Nameless.Microservices.App.Configs;

public static class ValidationConfig {
    extension(WebApplicationBuilder self) {
        public WebApplicationBuilder ConfigureValidation(Assembly[] assemblies) {
            // Configures the FluentValidation infrastructure. This will
            // make available the service IValidationService throughout the
            // application. Also, it will automatically register all classes
            // that implements IValidator interface.
            self.Services.RegisterValidation(options => {
                options.Assemblies = assemblies;
            });

            return self;
        }
    }
}
