using Nameless.Web.Endpoints.Definitions;

namespace Nameless.Web.Endpoints.Infrastructure;

/// <summary>
///     Endpoint factory interface.
/// </summary>
public interface IEndpointFactory {
    /// <summary>
    ///     Creates an instance of the specified endpoint type.
    /// </summary>
    /// <param name="descriptor">
    ///     The endpoint descriptor.
    /// </param>
    /// <returns>
    ///     An instance of the endpoint given by the descriptor.
    /// </returns>
    IEndpoint Create(IEndpointDescriptor descriptor);
}