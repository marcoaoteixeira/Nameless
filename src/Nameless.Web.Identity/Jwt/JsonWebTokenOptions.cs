using System.Security.Claims;

namespace Nameless.Web.Identity.Jwt;

/// <summary>
///     JSON Web Token options.
/// </summary>
public sealed class JsonWebTokenOptions {
    /// <summary>
    ///     Gets or sets the secret used to encrypt/validate the token.
    /// </summary>
    public string? Secret { get; set; }

    /// <summary>
    ///     Gets or sets the additional claims to include in the token.
    /// </summary>
    public Claim[] AdditionalClaims { get; set; } = [];

    /// <summary>
    ///     Gets or sets the issuer.
    /// </summary>
    public string? Issuer { get; set; }

    /// <summary>
    ///     Whether it should validate the issuer.
    /// </summary>
    public bool ValidateIssuer { get; set; }

    /// <summary>
    ///     Whether it should validate the issuer signin key.
    /// </summary>
    public bool ValidateIssuerSigninKey { get; set; }

    /// <summary>
    ///     Gets or sets the audience.
    /// </summary>
    public string[] Audiences { get; set; } = [];

    /// <summary>
    ///     Whether it should validate the audience.
    /// </summary>
    public bool ValidateAudience { get; set; }

    /// <summary>
    ///     Whether it should validate the token lifetime.
    /// </summary>
    public bool ValidateLifetime { get; set; }

    /// <summary>
    ///     Gets or sets the access token time-to-live.
    /// </summary>
    public TimeSpan TokenExpiresIn { get; set; } = TimeSpan.FromMinutes(minutes: 45);

    /// <summary>
    ///     Gets or sets the clock skew.
    /// </summary>
    public TimeSpan ClockSkew { get; set; } = TimeSpan.FromMinutes(minutes: 5);
}