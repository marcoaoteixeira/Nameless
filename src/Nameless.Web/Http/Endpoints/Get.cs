using Nameless.Web.Http.Endpoints.Attributes;

namespace Nameless.Web.Http.Endpoints;

/// <summary>
///     Represents the HTTP GET method.
///     Use as the type argument of <see cref="EndpointAttribute{THttpVerb}"/>
///     to declare a GET endpoint.
/// </summary>
public sealed class Get : IHttpVerb {
    /// <inheritdoc/>
    public static string Method => "GET";
}
