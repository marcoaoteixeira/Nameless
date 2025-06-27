namespace Nameless.Identity.Web.Configs;

public static class SecurityConfig {
    public static WebApplicationBuilder ConfigureSecurityServices(this WebApplicationBuilder self) {
        self.Services.AddAntiforgery();

        return self;
    }

    public static WebApplication UseSecurityServices(this WebApplication self) {
        if (!self.Environment.IsDevelopment()) {
            self.UseHsts();
        }

        self.UseHttpsRedirection()
            .UseAntiforgery();

        return self;
    }
}
