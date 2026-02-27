namespace Nameless.Web.MinimalEndpoints.Definitions.Metadata;

/// <summary>
///     Defines metadata for request acceptance in an endpoint.
/// </summary>
/// <param name="RequestType">The request type.</param>
/// <param name="IsOptional">Whether it is optional.</param>
/// <param name="ContentTypes">The content types.</param>
public record AcceptMetadata(Type RequestType, bool IsOptional, string[] ContentTypes);