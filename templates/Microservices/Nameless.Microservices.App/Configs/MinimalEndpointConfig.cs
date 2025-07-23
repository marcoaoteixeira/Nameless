using System.Reflection;
using Nameless.Web.Endpoints;
using Nameless.Web.OpenApi;

namespace Nameless.Microservices.App.Configs;

public static class MinimalEndpointConfig {
    public static WebApplicationBuilder ConfigureMinimalEndpoints(this WebApplicationBuilder self, Assembly[] assemblies) {
        self.RegisterMinimalEndpoints(options => {
            options.Assemblies = assemblies;
            options.ConfigureOpenApi = ConfigureOpenApi;
        });

        return self;
    }

    private static IEnumerable<OpenApiDescriptor> ConfigureOpenApi() {
        yield return new OpenApiDescriptor {
            DocumentName = "v1",
            Options = options => {
                options.AddDocumentTransformer<BearerSecuritySchemeDocumentTransformer>();
                options.AddOperationTransformer<DeprecatedOperationTransformer>();
            }
        };
    }
}
