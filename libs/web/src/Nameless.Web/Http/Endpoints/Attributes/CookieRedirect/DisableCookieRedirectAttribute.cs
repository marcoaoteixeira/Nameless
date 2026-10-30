namespace Nameless.Web.Http.Endpoints.Attributes.CookieRedirect;

/// <summary>
///     Specifies that cookie-based authentication redirects are disabled for
///     the endpoint.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class DisableCookieRedirectAttribute : Attribute;