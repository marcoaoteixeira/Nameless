using Microsoft.AspNetCore.Http;
using Nameless.Attributes;

namespace Nameless.Web.RequestTimeout;

[ConfigurationSectionName("RequestTimeoutPolicies")]
public record RequestTimeoutPolicyOptions {
    /// <summary>
    ///     Gets the timeout to apply to the policy.
    /// </summary>
    public TimeSpan ExpiresIn { get; init; }
    
    /// <summary>
    ///     Gets the response HTTP status code.
    ///     Default is <see cref="StatusCodes.Status408RequestTimeout"/>.
    /// </summary>
    public int HttpStatusCode { get; init; } = StatusCodes.Status408RequestTimeout;

    /// <summary>
    ///     Whether it should apply the policy.
    /// </summary>
    public bool Skip { get; init; }
}