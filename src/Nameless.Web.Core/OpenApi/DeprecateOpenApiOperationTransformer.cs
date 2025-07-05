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
        var stability = context.Description.ActionDescriptor.EndpointMetadata.OfType<Stability>().LastOrDefault();

        if (stability == Stability.Stable) {
            return Task.CompletedTask;
        }

        operation.Extensions.TryAdd(
            key: "x-scalar-stability",
            value: new OpenApiString(stability.ToString())
        );

        return Task.CompletedTask;
    }
}
