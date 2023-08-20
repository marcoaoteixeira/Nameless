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
        /// <summary>
        /// Gets or sets the token time-to-live in hours.
        /// Default is <c>24 hours</c>.
        /// </summary>
        public int AccessTokenTtl { get; set; } = 24;
        public bool ValidateLifetime { get; set; }
        /// <summary>
        /// Gets or sets the refresh token time-to-live in hours.
        /// Default is <c>24 days</c>.
        /// </summary>
        public int RefreshTokenTtl { get; set; } = 24;
        public bool RequireHttpsMetadata { get; set; }
        /// <summary>
        /// Gets or sets whether to validate issuer signing key.
        /// Default is <c>true</c>.
        /// </summary>
        public bool ValidateIssuerSigningKey { get; set; } = true;
        /// <summary>
        /// Gets or sets the maximum allowable time difference between client and server clock settings in seconds.
        /// Default is <c>0</c>.
        /// </summary>
        public int MaxClockSkew { get; set; }
        public bool SaveTokens { get; set; } = true;

        #endregion
    }
}
