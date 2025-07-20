using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Nameless.Web.Endpoints;

namespace Nameless.Web.OpenApi;

/// <summary>
///     Deprecates OpenAPI operations based on the stability of the endpoint.
/// </summary>
public sealed class DeprecateOpenApiOperationTransformer : IOpenApiOperationTransformer {
    /// <inheritdoc />
    /// <remarks>
    ///     It tries to simulate the behavior of the `StabilityOpenApiOperationFilter` from the `Scalar.AspNetCore` package,
    /// </remarks>
    public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken) {
        var stability = context.Description
                               .ActionDescriptor
                               .EndpointMetadata
                               .OfType<Stability>()
                               .LastOrDefault();

        operation.Deprecated = stability == Stability.Deprecated;

        var value = new OpenApiString(stability.ToString());
        var keys = new[] { "x-stability", "x-scalar-stability" };

        foreach (var key in keys) {
            operation.Extensions.TryAdd(key, value);
        }

        return Task.CompletedTask;
    }
}
