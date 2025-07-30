namespace Nameless.Web.Endpoints.Definitions;

/// <summary>
///     Defines metadata for request acceptance in an endpoint.
/// </summary>
/// <param name="RequestType">
///     The request type that the endpoint accepts.
/// </param>
/// <param name="IsOptional">
///     Whether the request is optional.
/// </param>
/// <param name="ContentTypes">
///     The content types that the endpoint accepts.
/// </param>
public record AcceptMetadata(Type RequestType, bool IsOptional, string[] ContentTypes);