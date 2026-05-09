using Nameless.Web.Http.Endpoints.Attributes;

namespace Nameless.Web.Http.Endpoints;

/// <summary>
///     Represents the HTTP HEAD method.
///     Use as the type argument of <see cref="EndpointAttribute{THttpVerb}"/>
///     to declare a HEAD endpoint.
/// </summary>
public sealed class Head : IHttpVerb {
    /// <inheritdoc/>
    public static string Method => "HEAD";
}
