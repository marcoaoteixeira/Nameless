using Nameless.Web.Endpoints;

namespace Nameless.Identity.Web.Configs;

public static class EndpointsConfig {
    public static WebApplication UseMinimalEndpointServices(this WebApplication self) {
        self.UseRouting();
        self.UseOutputCache();
        self.UseAuthorization();
        self.UseMinimalEndpoints();

        return self;
    }
}
