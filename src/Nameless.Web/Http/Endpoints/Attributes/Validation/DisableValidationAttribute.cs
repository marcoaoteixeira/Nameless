namespace Nameless.Web.Http.Endpoints.Attributes.Validation;

/// <summary>
///     Disables validation for the specified endpoint.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class DisableValidationAttribute : Attribute;