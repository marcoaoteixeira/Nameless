using System.Reflection;
using Nameless.Web.Endpoints;
using Nameless.Web.Endpoints.Interception;
using Nameless.Web.OpenApi;

namespace Nameless.Microservices.App.Configs;

public static class MinimalEndpointConfig {
    public static WebApplicationBuilder ConfigureMinimalEndpoints(this WebApplicationBuilder self, Assembly[] assemblies) {
        self.RegisterMinimalEndpoints(useInterception: true, options => {
            options.Assemblies = assemblies;
            options.ConfigureOpenApi = ConfigureOpenApi;
        });

        return self;
    }

    private static IEnumerable<OpenApiDocumentOptions> ConfigureOpenApi() {
        yield return new OpenApiDocumentOptions {
            DocumentName = "v1",
            Options = options => {
                options.AddDocumentTransformer<BearerSecuritySchemeDocumentTransformer>();
                options.AddOperationTransformer<DeprecatedOperationTransformer>();
            }
        };
    }
}
