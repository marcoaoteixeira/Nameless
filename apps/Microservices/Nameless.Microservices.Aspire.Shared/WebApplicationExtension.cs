using Microsoft.AspNetCore.Builder;
using Nameless.Web.HealthChecks;

namespace Nameless.Microservices.Aspire.Shared;

public static class WebApplicationExtension {
    public static WebApplication UseHealthCheckServices(this WebApplication self) {
        return self.UseHealthChecks(self.Environment);
    }
}
