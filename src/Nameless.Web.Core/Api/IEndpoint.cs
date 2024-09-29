namespace Nameless.Web.Api;

public interface IEndpoint {
    /// <summary>
    /// Gets the endpoint HTTP method.
    /// </summary>
    string HttpMethod { get; }

    /// <summary>
    /// Gets the endpoint route pattern.
    /// </summary>
    string RoutePattern { get; }

    /// <summary>
    /// Retrieves the endpoint request delegate handler.
    /// </summary>
    /// <returns>
    /// Endpoint's <see cref="Delegate"/> handler.
    /// </returns>
    Delegate GetHandler();
}