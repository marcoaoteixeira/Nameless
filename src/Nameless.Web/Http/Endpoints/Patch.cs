using Nameless.Web.Http.Endpoints.Attributes;

namespace Nameless.Web.Http.Endpoints;

/// <summary>
///     Represents the HTTP PATCH method.
///     Use as the type argument of <see cref="EndpointAttribute{THttpVerb}"/>
///     to declare a PATCH endpoint.
/// </summary>
public sealed class Patch : IHttpVerb {
    /// <inheritdoc/>
    public static string Method => "PATCH";
}
