namespace Nameless.Microservices.Web.Configs;

public static class SecurityConfig {
    public static WebApplicationBuilder ConfigureSecurity(this WebApplicationBuilder self) {
        self.Services.AddAntiforgery();

        return self;
    }

    public static WebApplication UseSecurity(this WebApplication self) {
        // Enables HSTS (HTTP Strict Transport Security) in non-development environments
        if (!self.Environment.IsDevelopment()) {
            self.UseHsts();
        }

        self.UseHttpsRedirection();

        return self;
    }
}
