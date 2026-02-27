using Nameless.Web.MinimalEndpoints.Definitions;

namespace Nameless.Web.MinimalEndpoints;

/// <summary>
///     Defines a minimal endpoint interface.
/// </summary>
public interface IEndpoint {
    /// <summary>
    ///     Describes the endpoint.
    /// </summary>
    /// <returns>
    ///     An <see cref="IEndpointDescriptor"/> that describes the endpoint.
    /// </returns>
    IEndpointDescriptor Describe();
}