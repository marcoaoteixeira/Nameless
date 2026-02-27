using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Nameless.Microservices.StartUp;

public static partial class WebAppExtensions {
    extension(WebApplicationBuilder self) {
        public WebApplicationBuilder CommonConfig() {
            // Configures common services for the application.
            // These configurations should be moved to their own
            // files if they grow larger or more complex.

            // Adds the IOptions<> pattern for configuration binding.
            self.Services.AddOptions();

            // Adds the ProblemDetails service for creating problem details responses.
            self.Services.AddProblemDetails();

            // Adds the HttpContextAccessor service to access the current HTTP context.
            self.Services.AddHttpContextAccessor();

            return self;
        }
    }
}
