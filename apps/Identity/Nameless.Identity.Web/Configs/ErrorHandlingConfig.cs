namespace Nameless.Identity.Web.Configs;

public static class ErrorHandlingConfig {
    public static WebApplication UseErrorHandlingServices(this WebApplication self) {
        self.UseExceptionHandler();

        return self;
    }
}
