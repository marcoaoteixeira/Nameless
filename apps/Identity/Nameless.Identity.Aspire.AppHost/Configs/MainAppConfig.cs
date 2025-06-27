using Projects;

namespace Nameless.Identity.Aspire.AppHost.Configs;

public static class MainAppConfig {
    public static IDistributedApplicationBuilder RegisterMainApplication(this IDistributedApplicationBuilder self) {
        self.AddProject<Nameless_Identity_Web>("main");

        return self;
    }
}
