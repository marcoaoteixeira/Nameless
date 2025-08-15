using Scalar.AspNetCore;

namespace Nameless.Microservices.App.Configs;

public static class OpenApiConfig {
    public static WebApplication UseOpenApi(this WebApplication self) {
        if (!self.Environment.IsDevelopment()) {
            return self;
        }

        self.MapOpenApi();
        self.MapScalarApiReference(options => {
            options.WithTitle(self.Environment.ApplicationName)
                   .WithTheme(ScalarTheme.BluePlanet)
                   .WithDefaultHttpClient(ScalarTarget.Shell, ScalarClient.Curl);
        });

        return self;
    }
}
