using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Nameless.Web;

public static class SwaggerUIOptionsExtension {
    public static void UseVersionableEndpoints(this SwaggerUIOptions options, IApplicationBuilder applicationBuilder) {
        var apiVersionDescriptionProvider = applicationBuilder.ApplicationServices
                                                              .GetService<IApiVersionDescriptionProvider>() ?? NullApiVersionDescriptionProvider.Instance;

        foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions) {
            options.SwaggerEndpoint(url: $"/swagger/{description.GroupName}/swagger.json",
                                    name: description.GroupName.ToUpperInvariant());
        }
    }
}
