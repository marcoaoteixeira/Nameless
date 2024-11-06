namespace Nameless.Web.Options;

public sealed record JwtOptions {
    private const string JWT_SECRET = "VGhlIG1vb24sIGEgY2VsZXN0aWFsIHBvZXQncyBwZWFybCwgYmF0aGVzIHRoZSBuaWdodCBjYW52YXMgaW4gYW4gZXRoZXJlYWwgZ2xvdywgd2hpc3BlcmluZyBhbmNpZW50IHNlY3JldHMgdG8gdGhlIHN0YXJnYXplcidzIHNvdWwsIGFuIGV0ZXJuYWwgZGFuY2Ugb2YgbGlnaHQgdGhhdCB3ZWF2ZXMgZHJlYW1zIGFjcm9zcyB0aGUgY29zbWljIHRhcGVzdHJ5Lg==";
    private const int DEFAULT_ACCESS_TOKEN_TTL = 60;
    private const int MAX_ACCESS_TOKEN_TTL = DEFAULT_ACCESS_TOKEN_TTL * 60;

    private string? _secret;
    public string Secret {
        get => _secret.WithFallback(JWT_SECRET);
        set => _secret = value;
    }

    public string? Issuer { get; set; }

    public bool ValidateIssuer { get; set; }

    public string? Audience { get; set; }

    public bool ValidateAudience { get; set; }

    private int _accessTokenTtl = 60;
    /// <summary>
    ///     Gets or sets the token time-to-live in minutes.
    ///     Default is <c>60 minutes</c>.
    ///     Note: Value should be between 1 and 1440 minutes (one day).
    /// </summary>
    public int AccessTokenTtl {
        get => _accessTokenTtl.IsBetween(min: 1, max: MAX_ACCESS_TOKEN_TTL)
            ? _accessTokenTtl
            : DEFAULT_ACCESS_TOKEN_TTL;
        set => _accessTokenTtl = value;
    }

    public bool ValidateLifetime { get; set; }

    /// <summary>
    ///     Gets or sets whether to validate issuer signing key.
    ///     Default is <c>true</c>.
    /// </summary>
    public bool ValidateIssuerSigningKey { get; set; } = true;

    /// <summary>
    ///     Gets or sets the maximum allowable time difference between client and server systemClock
    ///     settings in seconds.
    ///     Default is <c>0</c>.
    /// </summary>
    public int MaxClockSkew { get; set; }

    public bool SaveTokens { get; set; } = true;
}