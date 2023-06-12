namespace Nameless.WebApplication.Options {

    public sealed class JsonWebTokenOptions {

        #region Public Static Read-Only Properties

        public static JsonWebTokenOptions Default => new();

        #endregion

        #region Public Properties

        public string Secret { get; set; } = string.Empty;
        public string Issuer { get; set; } = "Issuer";
        public bool ValidateIssuer { get; set; }
        public string Audience { get; set; } = "Audience";
        public bool ValidateAudience { get; set; }
        /// <summary>
        /// Gets or sets the token time-to-live in seconds.
        /// Default is <c>15</c> minutes.
        /// </summary>
        public int AccessTokenTtl { get; set; } = Convert.ToInt32(TimeSpan.FromMinutes(15).TotalSeconds);
        public bool ValidateLifetime { get; set; }
        /// <summary>
        /// Gets or sets the refresh token time-to-live in seconds.
        /// Default is <c>15</c> days.
        /// </summary>
        public int RefreshTokenTtl { get; set; } = Convert.ToInt32(TimeSpan.FromDays(15).TotalSeconds);
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

        #endregion
    }
}
