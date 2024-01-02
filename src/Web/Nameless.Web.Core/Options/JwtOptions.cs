namespace Nameless.Web.Options {
    public sealed class JwtOptions {
        #region Public Static Read-Only Properties

        public static JwtOptions Default => new();

        #endregion

        #region Public Constructors

        public JwtOptions() {
            Secret = Environment.GetEnvironmentVariable(Root.EnvTokens.JWT_SECRET)
                ?? Root.Defaults.JWT_SECRET;
        }

        #endregion

        #region Public Properties

        public string Secret { get; }
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
            set => _accessTokenTtl = Guard.Against.OutOfRange(
                value: value,
                min: 1,
                max: 24 * 60,
                name: nameof(value)
            );
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
            set => _refreshTokenTtl = Guard.Against.OutOfRange(
                value: value,
                min: 1,
                max: 24 * 60,
                name: nameof(value)
            );
        }
        public bool RequireHttpsMetadata { get; set; }
        /// <summary>
        /// Gets or sets whether to validate issuer signing key.
        /// Default is <c>true</c>.
        /// </summary>
        public bool ValidateIssuerSigningKey { get; set; } = true;
        /// <summary>
        /// Gets or sets the maximum allowable time difference between client and server clock
        /// settings in seconds.
        /// Default is <c>0</c>.
        /// </summary>
        public int MaxClockSkew { get; set; }
        public bool SaveTokens { get; set; } = true;

        #endregion
    }
}
