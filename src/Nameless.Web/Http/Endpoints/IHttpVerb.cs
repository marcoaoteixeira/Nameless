using Nameless.Web.Http.Endpoints.Attributes;

namespace Nameless.Web.Http.Endpoints;

/// <summary>
///     Marker interface for HTTP verb token types used as the type argument of
///     <see cref="EndpointAttribute{THttpVerb}"/>.
/// </summary>
/// <remarks>
///     Use one of the built-in implementations (e.g., <see cref="Get"/>,
///     <see cref="Post"/>) or supply a custom type to add support for
///     non-standard HTTP methods.
/// </remarks>
public interface IHttpVerb {
    /// <summary>
    ///     Gets the HTTP method token as defined by RFC 9110
    ///     (e.g., <c>"GET"</c>, <c>"POST"</c>).
    /// </summary>
    static abstract string Method { get; }
}
