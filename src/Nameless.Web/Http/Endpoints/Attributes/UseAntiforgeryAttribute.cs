namespace Nameless.Web.Http.Endpoints.Attributes;

/// <summary>
///     Requires antiforgery token validation on the endpoint.
/// </summary>
/// <remarks>
///     The source generator emits
///     <c>.WithMetadata(new RequireAntiforgeryTokenAttribute())</c> on the
///     route handler builder when this attribute is present. Antiforgery
///     services must be registered via
///     <c>builder.Services.AddAntiforgery()</c> and the middleware added
///     via <c>app.UseAntiforgery()</c>. The antiforgery middleware must be
///     registered after <c>app.UseAuthentication()</c> and
///     <c>app.UseAuthorization()</c>.
/// </remarks>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class UseAntiforgeryAttribute : Attribute;
