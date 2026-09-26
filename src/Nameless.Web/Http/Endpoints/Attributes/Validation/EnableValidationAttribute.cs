namespace Nameless.Web.Http.Endpoints.Attributes.Validation;

/// <summary>
///     Enable validation for the specified endpoint.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class EnableValidationAttribute : Attribute;