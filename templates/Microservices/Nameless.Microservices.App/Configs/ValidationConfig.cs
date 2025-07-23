using System.Reflection;
using Nameless.Validation.FluentValidation;

namespace Nameless.Microservices.App.Configs;

public static class ValidationConfig {
    public static WebApplicationBuilder ConfigureValidation(this WebApplicationBuilder self, Assembly[] assemblies) {
        self.Services.RegisterValidation(options => {
            options.Assemblies = assemblies;
        });

        return self;
    }
}
