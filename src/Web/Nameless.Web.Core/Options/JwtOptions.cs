namespace Nameless.Web.Options;

public sealed record JwtOptions {
    public static JwtOptions Default => new();

    public string Secret { get; set; } = Root.Defaults.JWT_SECRET;
        
    public string? Issuer { get; set; }
        
    public bool ValidateIssuer { get; set; }
        
    public string? Audience { get; set; }
        
    public bool ValidateAudience { get; set; }

    private int _accessTokenTtl = 60;
    /// <summary>
    /// Gets or sets the token time-to-live in minutes.
    /// Default is <c>60 minutes</c>.
    /// Note: Value should be between 1 and 1440 minutes (one day).
    /// </summary>
    public int AccessTokenTtl {
        get => _accessTokenTtl;
        set => _accessTokenTtl = Prevent.Argument.OutOfRange(paramValue: value,
                                                             min: 1,
                                                             max: 24 * 60);
    }
    public bool ValidateLifetime { get; set; }

    private int _refreshTokenTtl = 60;
    /// <summary>
    /// Gets or sets the refresh token time-to-live in minutes.
    /// Default is <c>60 minutes</c>.
    /// Note: Value should be between 1 and 1440 minutes (one day).
    /// </summary>
    public int RefreshTokenTtl {
        get => _refreshTokenTtl;
        set => _refreshTokenTtl = Prevent.Argument.OutOfRange(paramValue: value,
                                                              min: 1,
                                                              max: 24 * 60);
    }
        
    public bool RequireHttpsMetadata { get; set; }
        
    /// <summary>
    /// Gets or sets whether to validate issuer signing key.
    /// Default is <c>true</c>.
    /// </summary>
    public bool ValidateIssuerSigningKey { get; set; } = true;
        
    /// <summary>
    /// Gets or sets the maximum allowable time difference between client and server systemClock
    /// settings in seconds.
    /// Default is <c>0</c>.
    /// </summary>
    public int MaxClockSkew { get; set; }
        
    public bool SaveTokens { get; set; } = true;
}