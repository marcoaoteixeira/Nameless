namespace Nameless.Web.Cors;

/// <summary>
///     Represents a CORS policy entry.
/// </summary>
/// <param name="Name">Name of the policy.</param>
/// <param name="Headers">A comma-separated list of allowed headers. Use "*" to allow any header.</param>
/// <param name="Methods">A comma-separated list of allowed methods (e.g.: such as GET, PUT, POST, DELETE). Use "*" to allow any method.</param>
/// <param name="Origins">A comma-separated list of allowed origins. Use "*" to allow any origin.</param>
/// <param name="SupportsCredentials">Whether it should validate credentials.</param>
public record CorsPolicyEntry(
    string Name,
    string Headers,
    string Methods,
    string Origins,
    bool SupportsCredentials,
    TimeSpan PreflightMaxAge
) {
    public static CorsPolicyEntry AllowEverything { get; } = new(
        Name: Defaults.CorsPolicies.ALLOW_EVERYTHING,
        Headers: "*",
        Methods: "*",
        Origins: "*",
        SupportsCredentials: false,
        PreflightMaxAge: TimeSpan.Zero
    );
}