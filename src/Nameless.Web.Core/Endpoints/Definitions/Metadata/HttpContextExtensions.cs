using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace Nameless.Web.Endpoints.Definitions.Metadata;

/// <summary>
///     Extensions for <see cref="HttpContext"/>.
/// </summary>
internal static class HttpContextExtensions {
    /// <summary>
    ///     The current <see cref="HttpContext"/>.
    /// </summary>
    extension(HttpContext self) {
        /// <summary>
        ///     Gets the <see cref="EndpointDescriptorMetadata"/> from the
        ///     current <see cref="HttpContext"/>.
        /// </summary>
        /// <returns>
        ///     The <see cref="IEndpointDescriptor"/> associated with the current
        ///     HttpContext.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        ///     if the HTTP context does not have an endpoint feature or
        ///     if the endpoint descriptor metadata is not available.
        /// </exception>
        internal EndpointDescriptorMetadata GetEndpointDescriptorMetadata() {
            if (self.Features[typeof(IEndpointFeature)] is not IEndpointFeature feature) {
                throw new InvalidOperationException(
                    message: "Endpoint feature is not available in the current HttpContext.");
            }

            var metadata = feature.Endpoint?.Metadata.GetMetadata<EndpointDescriptorMetadata>();
            if (metadata is null) {
                throw new InvalidOperationException(message: "Endpoint metadata is not available.");
            }

            return metadata;
        }
    }
}