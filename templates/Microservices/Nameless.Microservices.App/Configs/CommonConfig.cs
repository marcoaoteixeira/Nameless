using Nameless.Microservices.App.Infrastructure;
using Nameless.Web.Correlation;
using Nameless.Web.Identity.Jwt;

namespace Nameless.Microservices.App.Configs;

public static class CommonConfig {
    extension(WebApplicationBuilder self) {
        public WebApplicationBuilder ConfigureCommon() {
            // Configures common services for the application.
            // These configurations should be moved to their own
            // files if they grow larger or more complex.

            // Adds antiforgery services. Useful for protecting against CSRF attacks.
            self.Services.AddAntiforgery(options => {
                options.HeaderName = "X-CSRF-TOKEN";
                options.Cookie.Name = "X-CSRF-TOKEN";
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            });

            // Adds the GlobalExceptionHandler to handle exceptions globally.
            self.Services.AddExceptionHandler<GlobalExceptionHandler>();

            // Adds the IOptions<> pattern for configuration binding.
            self.Services.AddOptions();

            // Adds the ProblemDetails service for creating problem details responses.
            self.Services.AddProblemDetails();

            // Adds the HttpContextAccessor service to access the current HTTP context.
            self.Services.AddHttpContextAccessor();

            // Adds the configurations for the application.
            self.Services.Configure<JsonWebTokenOptions>(
                self.Configuration.GetSection(nameof(JsonWebTokenOptions))
            );
            self.Services.Configure<AppDataProtectionOptions>(
                self.Configuration.GetSection(nameof(AppDataProtectionOptions))
            );
            self.Services.Configure<HttpContextCorrelationOptions>(
                self.Configuration.GetSection(nameof(HttpContextCorrelationOptions))
            );

            return self;
        }
    }
}
