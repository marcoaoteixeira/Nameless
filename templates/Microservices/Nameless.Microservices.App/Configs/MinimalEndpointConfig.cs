using System.Reflection;
using Nameless.Web.Endpoints;
using Nameless.Web.OpenApi;

namespace Nameless.Microservices.App.Configs;

public static class MinimalEndpointConfig {
    extension(WebApplicationBuilder self) {
        public WebApplicationBuilder ConfigureMinimalEndpoints(Assembly[] assemblies) {
            // Registers the minimal endpoint infrastructure for the application.
            // This one is really important because it will enable everything
            // regarding minimal endpoints and how it is discovered inside the
            // application objects. Also, it's possible to configure other options
            // regarding OpenAPI, API Explorer and Versioning.
            self.RegisterMinimalEndpoints(options => {
                options.Assemblies = assemblies;
                options.ConfigureOpenApi = () => {
                    return
                    [
                        new OpenApiDocumentOptions
                        {
                            DocumentName = "v1",
                            Options = opts =>
                            {
                                opts.AddDocumentTransformer<BearerSecuritySchemeDocumentTransformer>();
                                opts.AddOperationTransformer<DeprecateOpenApiOperationTransformer>();
                            }
                        }
                    ];
                };
            });

            return self;
        }
    }
}
