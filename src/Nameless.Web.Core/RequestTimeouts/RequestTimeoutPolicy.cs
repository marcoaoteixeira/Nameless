using Microsoft.AspNetCore.Http;

namespace Nameless.Web.RequestTimeouts;

/// <summary>
///     Represents a request timeout policy entry.
/// </summary>
/// <param name="Name">Name of the policy.</param>
/// <param name="ExpiresIn">The timeout to apply to the policy.</param>
/// <param name="HttpStatusCode">The response HTTP status code. Default is <see cref="StatusCodes.Status408RequestTimeout"/>.</param>
public record RequestTimeoutPolicy(string Name, TimeSpan ExpiresIn = default, int HttpStatusCode = StatusCodes.Status408RequestTimeout) {
    public static RequestTimeoutPolicy OneSecond { get; } = new(
        Name: Defaults.RequestTimeoutPolicies.ONE_SECOND,
        ExpiresIn: TimeSpan.FromSeconds(1)
    );

    public static RequestTimeoutPolicy FiveSeconds { get; } = new(
        Name: Defaults.RequestTimeoutPolicies.FIVE_SECONDS,
        ExpiresIn: TimeSpan.FromSeconds(5)
    );

    public static RequestTimeoutPolicy FifteenSeconds { get; } = new(
        Name: Defaults.RequestTimeoutPolicies.FIFTEEN_SECONDS,
        ExpiresIn: TimeSpan.FromSeconds(15)
    );

    public static RequestTimeoutPolicy ThirtySeconds { get; } = new(
        Name: Defaults.RequestTimeoutPolicies.THIRTY_SECONDS,
        ExpiresIn: TimeSpan.FromSeconds(30)
    );

    public static RequestTimeoutPolicy OneMinute { get; } = new(
        Name: Defaults.RequestTimeoutPolicies.ONE_MINUTE,
        ExpiresIn: TimeSpan.FromMinutes(1)
    );
}