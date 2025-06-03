namespace Nameless.Web.Endpoints;

/// <summary>
/// Represents a minimal endpoint interface.
/// </summary>
public interface IEndpoint {
    /// <summary>
    /// Configures the endpoint with the specified builder.
    /// </summary>
    /// <param name="builder">The endpoint builder.</param>
    void Configure(IEndpointBuilder builder);
}