using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace Nameless.Web.Endpoints.Definitions;
internal static class HttpContextExtensions {
    internal static IEndpointDescriptor GetEndpointDescriptor(this HttpContext self) {
        Prevent.Argument.Null(self);

        if (self.Features[typeof(IEndpointFeature)] is not IEndpointFeature feature) {
            throw new InvalidOperationException("Endpoint feature is not available in the current HttpContext.");
        }

        var metadata = feature.Endpoint?.Metadata.GetMetadata<EndpointDescriptorMetadata>();
        if (metadata is null) {
            throw new InvalidOperationException("Endpoint descriptor metadata is not available.");
        }

        return metadata.Descriptor;
    }
}
