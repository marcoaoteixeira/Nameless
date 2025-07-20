namespace Nameless.Web.IdentityModel;

/// <summary>
///     JSON Web Token options.
/// </summary>
public record JsonWebTokenOptions {
    /// <summary>
    ///     Gets or sets the secret used to encrypt/validate the token.
    /// </summary>
    public string? Secret { get; set; }

    /// <summary>
    ///     Gets or sets the encryption algorithm.
    /// </summary>
    public string? Algorithm { get; set; }

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
    public string? Audience { get; set; }

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
    public TimeSpan AccessTokenTtl { get; set; } = TimeSpan.FromMinutes(45);

    /// <summary>
    ///     Gets or sets the clock skew.
    /// </summary>
    public TimeSpan ClockSkew { get; set; } = TimeSpan.FromMinutes(5);
}
