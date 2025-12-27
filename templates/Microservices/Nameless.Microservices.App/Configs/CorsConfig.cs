namespace Nameless.Microservices.App.Configs;

public static class CorsConfig {
    extension(WebApplicationBuilder self) {
        public WebApplicationBuilder ConfigureCors() {
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
}
