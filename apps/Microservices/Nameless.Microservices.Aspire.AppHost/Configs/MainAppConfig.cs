using Projects;

namespace Nameless.Microservices.Aspire.AppHost.Configs;

public static class MainAppConfig {
    public static IDistributedApplicationBuilder RegisterMainApplication(this IDistributedApplicationBuilder self) {
        self.AddProject<Nameless_Microservices_Web>("main");

        return self;
    }
}
