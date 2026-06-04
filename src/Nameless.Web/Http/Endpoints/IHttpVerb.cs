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

/// <summary>
///     Represents the HTTP DELETE method.
///     Use as the type argument of <see cref="EndpointAttribute{THttpVerb}"/>
///     to declare a DELETE endpoint.
/// </summary>
public sealed class Delete : IHttpVerb {
    /// <inheritdoc/>
    public static string Method => "DELETE";
}

/// <summary>
///     Represents the HTTP GET method.
///     Use as the type argument of <see cref="EndpointAttribute{THttpVerb}"/>
///     to declare a GET endpoint.
/// </summary>
public sealed class Get : IHttpVerb {
    /// <inheritdoc/>
    public static string Method => "GET";
}

/// <summary>
///     Represents the HTTP HEAD method.
///     Use as the type argument of <see cref="EndpointAttribute{THttpVerb}"/>
///     to declare a HEAD endpoint.
/// </summary>
public sealed class Head : IHttpVerb {
    /// <inheritdoc/>
    public static string Method => "HEAD";
}

/// <summary>
///     Represents the HTTP OPTIONS method.
///     Use as the type argument of <see cref="EndpointAttribute{THttpVerb}"/>
///     to declare an OPTIONS endpoint.
/// </summary>
public sealed class Options : IHttpVerb {
    /// <inheritdoc/>
    public static string Method => "OPTIONS";
}

/// <summary>
///     Represents the HTTP PATCH method.
///     Use as the type argument of <see cref="EndpointAttribute{THttpVerb}"/>
///     to declare a PATCH endpoint.
/// </summary>
public sealed class Patch : IHttpVerb {
    /// <inheritdoc/>
    public static string Method => "PATCH";
}

/// <summary>
///     Represents the HTTP POST method.
///     Use as the type argument of <see cref="EndpointAttribute{THttpVerb}"/>
///     to declare a POST endpoint.
/// </summary>
public sealed class Post : IHttpVerb {
    /// <inheritdoc/>
    public static string Method => "POST";
}

/// <summary>
///     Represents the HTTP PUT method.
///     Use as the type argument of <see cref="EndpointAttribute{THttpVerb}"/>
///     to declare a PUT endpoint.
/// </summary>
public sealed class Put : IHttpVerb {
    /// <inheritdoc/>
    public static string Method => "PUT";
}