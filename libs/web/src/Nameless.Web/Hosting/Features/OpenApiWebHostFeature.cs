using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nameless.Helpers;
using Nameless.Hosting;
using Nameless.Web.Hosting.Options;
using Nameless.Web.OpenApi;

namespace Nameless.Web.Hosting.Features;

/// <summary>
///     Registers OpenAPI and maps its endpoint.
/// </summary>
public class OpenApiWebHostFeature : HostFeature<OpenApiFeatureOptions>, IWebHostFeature {
    /// <inheritdoc />
    public override void Register(IHostApplicationBuilder builder, HostFeatureContext context) {
        var options = ActionHelper.FromDelegate(ConfigureFeature);

        if (options.DocumentOptions.Count == 0) {
            options.WithOpenApiDocument(
                document: "v1",
                options: opts => opts.AddDocumentTransformer<BearerSecuritySchemeDocumentTransformer>()
                                     .AddDocumentTransformer<HttpQueryOperationDocumentTransformer>()
            );
        }

        foreach (var document in options.DocumentOptions) {
            if (!string.IsNullOrWhiteSpace(document.Key)) {
                builder.Services.AddOpenApi(document.Key, document.Value);
            } else {
                builder.Services.AddOpenApi(document.Value);
            }
        }
    }

    /// <inheritdoc />
    public void Use(WebApplication app, HostFeatureContext context) {
        app.MapOpenApi();
    }
}
