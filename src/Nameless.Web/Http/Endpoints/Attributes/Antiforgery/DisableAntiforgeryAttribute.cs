namespace Nameless.Web.Http.Endpoints.Attributes.Antiforgery;

/// <summary>
///     Disables cross-site request forgery (CSRF) protection for the endpoint.
///     This should only be used when an endpoint is not vulnerable to CSRF
///     attacks.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class DisableAntiforgeryAttribute : Attribute;