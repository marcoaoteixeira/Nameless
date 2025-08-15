using System.Reflection;

namespace Nameless.Web.Endpoints.Infrastructure;

/// <summary>
///     Represents the endpoint information after being constructed.
/// </summary>
public record EndpointCall {
    /// <summary>
    ///     Gets the endpoint instance.
    /// </summary>
    public IEndpoint Target { get; init; }

    /// <summary>
    ///     Gets the endpoint handler method.
    /// </summary>
    public MethodInfo Handler { get; init; }

    /// <summary>
    ///     Initializes a new instance of <see cref="EndpointCall"/> class.
    /// </summary>
    /// <param name="target">
    ///     The endpoint instance.
    /// </param>
    /// <param name="handler">
    ///     The endpoint handler method.
    /// </param>
    public EndpointCall(IEndpoint target, MethodInfo handler) {
        Target = Guard.Against.Null(target);
        Handler = Guard.Against.Null(handler);
    }
}