using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Nameless.Web.Endpoints.Definitions;
using Nameless.Web.Helpers;

namespace Nameless.Web.Endpoints.Infrastructure;

/// <summary>
///     Extension methods for <see cref="RouteHandlerBuilder"/> .
/// </summary>
internal static class RouteHandlerBuilderExtensions {
    // This method will infer the metadata for the endpoint request type.
    // So we can get a nice OpenApi documentation for the endpoint.
    internal static void WithRequestMetadata(this RouteHandlerBuilder self, IEndpointDescriptor descriptor) {
        if (descriptor.Action is null) {
            throw new InvalidOperationException("Endpoint handler not found.");
        }

        var metadata = RequestDelegateFactory.InferMetadata(
            methodInfo: descriptor.Action,
            options: new RequestDelegateFactoryOptions {
                RouteParameterNames = RouteHelper.GetRouteParameters(descriptor.RoutePattern)
            }
        );

        foreach (var obj in metadata.EndpointMetadata) {
            self.WithMetadata(obj);
        }
    }
}
