using Scalar.AspNetCore;

namespace Nameless.Microservices.App.Configs;

public static class OpenApiConfig {
    public static WebApplication UseOpenApi(this WebApplication self) {
        self.MapOpenApi();

        // Do not expose the Scalar on Production env.
        if (self.Environment.IsDevelopment()) {
            self.MapScalarApiReference(options => {
                options
                    .WithTitle(self.Environment.ApplicationName)
                    .WithTheme(ScalarTheme.BluePlanet)
                    .WithDefaultHttpClient(ScalarTarget.Shell, ScalarClient.Curl);
            });
        }

        return self;
    }
}
