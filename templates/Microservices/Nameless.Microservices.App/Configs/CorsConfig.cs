namespace Nameless.Microservices.App.Configs;

public static class CorsConfig {
    public static WebApplicationBuilder ConfigureCors(this WebApplicationBuilder self) {
        // Adds CORS default policy
        self.Services.AddCors(options => {
            options.AddPolicy(Constants.CorsPolicies.ALLOW_EVERYTHING, builder => {
                builder.AllowAnyHeader()
                       .AllowAnyMethod()
                       .AllowAnyOrigin();
            });
        });

        return self;
    }
}
