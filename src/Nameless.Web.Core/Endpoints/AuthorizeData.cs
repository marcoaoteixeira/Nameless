using Microsoft.AspNetCore.Authorization;

namespace Nameless.Web.Endpoints;

/// <summary>
/// Represents a set of information about endpoint authorization.
/// </summary>
public sealed record AuthorizeData : IAuthorizeData {
    /// <summary>
    /// Gets or sets the authentication schemes.
    /// </summary>
    public string? AuthenticationSchemes { get; set; }
    /// <summary>
    /// Gets or sets the policy.
    /// </summary>
    public string? Policy { get; set; }
    /// <summary>
    /// Gets or sets the roles.
    /// </summary>
    public string? Roles { get; set; }

    /// <summary>
    /// Creates an <see cref="AuthorizeData"/> with the specified parameters.
    /// </summary>
    /// <param name="policy">The policy, must be non-null, not empty or white spaces.</param>
    /// <param name="authenticationSchemes">The authentication schemes.</param>
    /// <param name="roles">The roles.</param>
    /// <returns>
    /// A new instance of <see cref="AuthorizeData"/>.
    /// </returns>
    /// 
    public static IAuthorizeData For(string policy, string? authenticationSchemes = null, string? roles = null)
        => new AuthorizeData {
            Policy = Prevent.Argument.NullOrWhiteSpace(policy),
            Roles = roles,
            AuthenticationSchemes = authenticationSchemes
        };
}
