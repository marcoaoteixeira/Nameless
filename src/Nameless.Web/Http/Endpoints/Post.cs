using Nameless.Web.Http.Endpoints.Attributes;

namespace Nameless.Web.Http.Endpoints;

/// <summary>
///     Represents the HTTP POST method.
///     Use as the type argument of <see cref="EndpointAttribute{THttpVerb}"/>
///     to declare a POST endpoint.
/// </summary>
public sealed class Post : IHttpVerb {
    /// <inheritdoc/>
    public static string Method => "POST";
}
