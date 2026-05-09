using Nameless.Web.Http.Endpoints.Attributes;

namespace Nameless.Web.Http.Endpoints;

/// <summary>
///     Represents the HTTP PUT method.
///     Use as the type argument of <see cref="EndpointAttribute{THttpVerb}"/>
///     to declare a PUT endpoint.
/// </summary>
public sealed class Put : IHttpVerb {
    /// <inheritdoc/>
    public static string Method => "PUT";
}
