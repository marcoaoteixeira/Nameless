using Nameless.Web.Endpoints;

namespace Nameless.Microservices.Web.Configs;

public static class EndpointsConfig {
    public static WebApplicationBuilder RegisterMinimalEndpoints(this WebApplicationBuilder self) {
        var services = self.Services;

        services
           .AddOpenApi()
           .RegisterMinimalEndpoints(options => {
               options.Assemblies = [typeof(EndpointsConfig).Assembly];
           });

        return self;
    }

    public static WebApplication ResolveMinimalEndpoints(this WebApplication self) {
        self.MapOpenApi();
        self.UseRouting()
            .UseAuthorization()
            .UseAuthentication()
            .UseMinimalEndpoints();

        return self;
    }
}
