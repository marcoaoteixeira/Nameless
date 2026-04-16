namespace Nameless.Web.Endpoints.Definitions.Metadata;

/// <summary>
///     Represents metadata for the response produced by an endpoint.
/// </summary>
/// <param name="ResponseType">The response type that the endpoint produces.</param>
/// <param name="StatusCode">The HTTP status code returned by the endpoint.</param>
/// <param name="ContentTypes">The content types that the endpoint produces.</param>
public record ProduceMetadata(Type ResponseType, int StatusCode, string[] ContentTypes);