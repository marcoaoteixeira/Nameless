using Microsoft.AspNetCore.Authorization;

namespace Nameless.Web.Endpoints;

public sealed record AuthorizeData : IAuthorizeData {
    public string? Policy { get; set; }
    public string? Roles { get; set; }
    public string? AuthenticationSchemes { get; set; }

    public static IAuthorizeData For(string policy, string? authenticationSchemes = null, string? roles = null)
        => new AuthorizeData {
            Policy = Prevent.Argument.NullOrWhiteSpace(policy),
            Roles = roles,
            AuthenticationSchemes = authenticationSchemes
        };
}
