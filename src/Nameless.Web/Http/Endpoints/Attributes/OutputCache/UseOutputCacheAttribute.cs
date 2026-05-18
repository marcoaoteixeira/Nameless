namespace Nameless.Web.Http.Endpoints.Attributes.OutputCache;

/// <summary>
///     Applies the specified output cache policy to the endpoint or group.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class UseOutputCacheAttribute(string policyName) : Attribute {
    /// <summary>
    ///     Gets the policy name
    /// </summary>
    public string? PolicyName { get; } = policyName;
}