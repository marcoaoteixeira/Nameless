using Nameless.Attributes;

namespace Nameless.Microservices.Infrastructure.Auth;

/// <summary>
///     JSON Web Token options.
/// </summary>
[ConfigurationSectionName("JsonWebToken")]
public sealed class JsonWebTokenOptions {
    /// <summary>
    ///     Gets or sets the authorities.
    /// </summary>
    public string? Authority { get; set; }

    /// <summary>
    ///     Gets or sets the issuer.
    /// </summary>
    public string[] Issuers { get; set; } = [];

    /// <summary>
    ///     Whether it should validate the issuer.
    /// </summary>
    public bool ValidateIssuer { get; set; }

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
    ///     Whether it should validate the issuer signin key.
    /// </summary>
    public bool ValidateIssuerSigninKey { get; set; }

    /// <summary>
    ///     Gets or sets the clock skew.
    /// </summary>
    public TimeSpan ClockSkew { get; set; } = TimeSpan.FromSeconds(30);
}