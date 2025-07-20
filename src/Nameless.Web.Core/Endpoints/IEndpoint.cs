namespace Nameless.Web.Endpoints;

/// <summary>
/// Represents a minimal endpoint interface.
/// </summary>
public interface IEndpoint {
    /// <summary>
    /// Configures the endpoint with the specified descriptor.
    /// </summary>
    /// <param name="descriptor">The endpoint descriptor.</param>
    void Configure(IEndpointDescriptor descriptor);
}