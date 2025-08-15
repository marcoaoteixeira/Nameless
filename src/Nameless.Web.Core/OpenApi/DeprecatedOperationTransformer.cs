using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Nameless.Web.Endpoints.Definitions.Metadata;

namespace Nameless.Web.OpenApi;

/// <summary>
///     Deprecates OpenAPI operations based on the stability of the endpoint.
/// </summary>
public sealed class DeprecatedOperationTransformer : IOpenApiOperationTransformer {
    /// <inheritdoc />
    /// <remarks>
    ///     It tries to simulate the behavior of the `StabilityOpenApiOperationFilter` from the `Scalar.AspNetCore` package,
    /// </remarks>
    public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken) {
        var version = context.Description
                             .ActionDescriptor
                             .EndpointMetadata
                             .OfType<VersionMetadata>()
                             .LastOrDefault();

        operation.Deprecated = version.Stability == Stability.Deprecated;

        var value = new OpenApiString(version.Stability.ToString());
        var keys = new[] { "x-stability", "x-scalar-stability" };

        foreach (var key in keys) {
            operation.Extensions.TryAdd(key, value);
        }

        return Task.CompletedTask;
    }
}
