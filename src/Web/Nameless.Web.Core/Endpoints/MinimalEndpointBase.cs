namespace Nameless.Web.Endpoints;

/// <summary>
///     Base class to implement minimal API endpoints.
/// </summary>
public abstract class MinimalEndpointBase {
    /// <summary>
    ///     Gets the HTTP method for the endpoint. Default value is "GET".
    /// </summary>
    public virtual string HttpMethod => HttpMethods.GET;

    /// <summary>
    ///     Gets the endpoint route pattern.
    /// </summary>
    public abstract string RoutePattern { get; }

    /// <summary>
    ///     Creates the endpoint request handler delegate.
    /// </summary>
    /// <returns>
    ///     A <see cref="Delegate" /> that represents the endpoint request handler.
    /// </returns>
    public abstract Delegate CreateDelegate();

    /// <summary>
    ///     Configures the endpoint.
    /// </summary>
    /// <param name="builder">The route handler builder for this endpoint.</param>
    public virtual void Configure(IMinimalEndpointBuilder builder) {
        /* implement as needed */
    }
}