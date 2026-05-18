namespace Nameless.Web.Http.Endpoints.Attributes.OutputCache;

/// <summary>
///     Clears the policies and adds one preventing any caching logic to
///     happen.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class DisableOutputCacheAttribute : Attribute;