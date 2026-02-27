using System.Reflection;

namespace Nameless.Web.MinimalEndpoints.Definitions.Metadata;

/// <summary>
///     Represents an endpoint handler metadata.
/// </summary>
/// <param name="Handler">
///     The endpoint handler.
/// </param>
public record EndpointHandlerMetadata(MethodInfo Handler);