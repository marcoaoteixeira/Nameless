namespace Nameless.Web.Http.Endpoints.Attributes.CookieRedirect;

/// <summary>
///     Specifies that cookie-based authentication redirects
///     from the endpoint.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class AllowCookieRedirectAttribute : Attribute;