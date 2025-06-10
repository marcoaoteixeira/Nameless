using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace Nameless.Microservices.Infrastructure.OpenApi;

public sealed class DeprecateOpenApiOperationTransformer : IOpenApiOperationTransformer {
    public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken) {
        throw new NotImplementedException();
    }
}
