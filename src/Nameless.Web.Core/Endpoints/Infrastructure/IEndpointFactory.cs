namespace Nameless.Web.Endpoints.Infrastructure;

/// <summary>
///     Endpoint factory interface.
/// </summary>
public interface IEndpointFactory {
    /// <summary>
    ///     Creates an instance of the specified endpoint type.
    /// </summary>
    /// <param name="endpointType">
    ///     The endpoint type.
    /// </param>
    /// <returns>
    ///     An instance of the specified endpoint type,
    ///     cast to <see cref="IEndpoint"/>.
    /// </returns>
    IEndpoint Create(Type endpointType);
}