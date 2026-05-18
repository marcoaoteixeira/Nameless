namespace Nameless.Web.Http.Endpoints.Attributes.RateLimiting;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class UseRateLimitingAttribute(string policyName) : Attribute {
    /// <summary>
    ///     Gets the policy name
    /// </summary>
    public string PolicyName { get; } = policyName;
}