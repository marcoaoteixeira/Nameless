namespace Nameless.Web.Endpoints.Definitions.Metadata;

/// <summary>
///     Represents metadata for an endpoint descriptor.
/// </summary>
public record EndpointDescriptorMetadata {
    /// <summary>
    ///     Gets the endpoint descriptor.
    /// </summary>
    public IEndpointDescriptor Descriptor { get; }

    /// <summary>
    ///     Initializes a new instance of
    ///     <see cref="EndpointDescriptorMetadata"/> class.
    /// </summary>
    /// <param name="descriptor">
    ///     The endpoint descriptor.
    /// </param>
    public EndpointDescriptorMetadata(IEndpointDescriptor descriptor) {
        Descriptor = Guard.Against.Null(descriptor);
    }
}
