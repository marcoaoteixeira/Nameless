namespace Nameless.Web.Http.Endpoints.Attributes.RequestTimeout;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class UseRequestTimeoutAttribute(string policyName) : Attribute {
    /// <summary>
    ///     Gets the policy name
    /// </summary>
    public string? PolicyName { get; } = policyName;
}