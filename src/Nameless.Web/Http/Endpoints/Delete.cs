using Nameless.Web.Http.Endpoints.Attributes;

namespace Nameless.Web.Http.Endpoints;

/// <summary>
///     Represents the HTTP DELETE method.
///     Use as the type argument of <see cref="EndpointAttribute{THttpVerb}"/>
///     to declare a DELETE endpoint.
/// </summary>
public sealed class Delete : IHttpVerb {
    /// <inheritdoc/>
    public static string Method => "DELETE";
}
