using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using Microsoft.OpenApi.MicrosoftExtensions;
using Nameless.Web.Endpoints.Definitions.Metadata;

namespace Nameless.Web.OpenApi;

/// <summary>
///     Deprecates OpenAPI operations based on the stability of the endpoint.
/// </summary>
public class DeprecateOpenApiOperationTransformer : IOpenApiOperationTransformer {
    /// <inheritdoc />
    /// <remarks>
    ///     It tries to simulate the behavior of the StabilityOpenApiOperationFilter from the Scalar.AspNetCore package.
    /// </remarks>
    public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context,
        CancellationToken cancellationToken) {
        var deprecateVersionMetadata = context
                                       .Description
                                       .ActionDescriptor
                                       .EndpointMetadata
                                       .OfType<VersionMetadata>()
                                       .FirstOrDefault(metadata => metadata.Stability == Stability.Deprecated);

        if (deprecateVersionMetadata.Number == 0) {
            return Task.CompletedTask;
        }

        operation.Deprecated = true;
        operation.Extensions ??= new Dictionary<string, IOpenApiExtension>();

        var keys = new[] { "x-stability", "x-scalar-stability" };

        foreach (var key in keys) {
            operation.Extensions.TryAdd(key,
                new OpenApiDeprecationExtension {
                    Description = nameof(Stability.Deprecated), Version = deprecateVersionMetadata.Number.ToString()
                });
        }

        return Task.CompletedTask;
    }
}