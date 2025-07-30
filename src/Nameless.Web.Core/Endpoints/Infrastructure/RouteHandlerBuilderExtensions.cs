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
    internal static void WithEndpointDescriptorMetadata(this RouteHandlerBuilder self, IEndpointDescriptor descriptor) {
        var result = RequestDelegateFactory.InferMetadata(
            methodInfo: descriptor.GetAction(),
            options: new RequestDelegateFactoryOptions {
                RouteParameterNames = RouteHelper.GetRouteParameters(descriptor.RoutePattern)
            }
        );

        foreach (var metadata in result.EndpointMetadata) {
            self.WithMetadata(metadata);
        }

        self.WithMetadata(new EndpointDescriptorMetadata(descriptor));
    }
}
