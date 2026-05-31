namespace Nameless.Web.Http.Endpoints.Attributes.Cors;

/// <summary>
///     Provides metadata needed for enabling CORS support.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class UseCorsAttribute(string policyName) : Attribute {
    /// <summary>
    ///     Gets the policy name
    /// </summary>
    public string? PolicyName { get; } = policyName;
}