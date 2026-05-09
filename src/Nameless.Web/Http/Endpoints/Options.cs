using Nameless.Web.Http.Endpoints.Attributes;

namespace Nameless.Web.Http.Endpoints;

/// <summary>
///     Represents the HTTP OPTIONS method.
///     Use as the type argument of <see cref="EndpointAttribute{THttpVerb}"/>
///     to declare an OPTIONS endpoint.
/// </summary>
public sealed class Options : IHttpVerb {
    /// <inheritdoc/>
    public static string Method => "OPTIONS";
}
