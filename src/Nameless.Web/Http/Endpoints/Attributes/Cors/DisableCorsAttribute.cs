namespace Nameless.Web.Http.Endpoints.Attributes.Cors;

/// <summary>
///     Disable cors for a resource.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class DisableCorsAttribute : Attribute;