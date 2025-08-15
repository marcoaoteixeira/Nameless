using System.Reflection;

namespace Nameless.Web.Endpoints.Definitions.Metadata;

/// <summary>
///     Represents an endpoint handler metadata.
/// </summary>
public record EndpointHandlerMetadata {
    /// <summary>
    ///     Gets the handler for the endpoint.
    /// </summary>
    public MethodInfo Handler { get; }

    /// <summary>
    ///     Initializes a new instance of <see cref="EndpointHandlerMetadata"/>
    ///     class.
    /// </summary>
    /// <param name="handler">
    ///     The handler for the endpoint.
    /// </param>
    public EndpointHandlerMetadata(MethodInfo handler) {
        Handler = Guard.Against.Null(handler);
    }
}
