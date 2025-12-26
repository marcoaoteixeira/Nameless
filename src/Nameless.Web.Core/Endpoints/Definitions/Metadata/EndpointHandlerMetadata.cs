using System.Reflection;

namespace Nameless.Web.Endpoints.Definitions.Metadata;

/// <summary>
///     Represents an endpoint handler metadata.
/// </summary>
/// <param name="Handler">
///     The endpoint handler.
/// </param>
public sealed record EndpointHandlerMetadata(MethodInfo Handler);