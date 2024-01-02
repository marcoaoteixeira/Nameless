using System.ComponentModel;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Nameless.Web.Services {
    public sealed record JwtClaims {
        #region Public Properties

        /// <summary>
        /// Gets or sets the subject identifier. <see cref="JwtRegisteredClaimNames.Sub"/>.
        /// </summary>
        [Description(JwtRegisteredClaimNames.Sub)]
        public string? Sub { get; init; }

        /// <summary>
        /// Gets or sets the subject name. <see cref="JwtRegisteredClaimNames.Name"/>.
        /// </summary>
        [Description(JwtRegisteredClaimNames.Name)]
        public string? Name { get; init; }

        /// <summary>
        /// Gets or sets the subject email. <see cref="JwtRegisteredClaimNames.Email"/>.
        /// </summary>
        [Description(JwtRegisteredClaimNames.Email)]
        public string? Email { get; init; }

        /// <summary>
        /// Gets or sets the subject birthdate. <see cref="JwtRegisteredClaimNames.Birthdate"/>.
        /// </summary>
        [Description(JwtRegisteredClaimNames.Birthdate)]
        public string? Birthdate { get; init; }

        /// <summary>
        /// Gets or sets the subject gender. <see cref="JwtRegisteredClaimNames.Gender"/>.
        /// </summary>
        [Description(JwtRegisteredClaimNames.Gender)]
        public string? Gender { get; init; }

        /// <summary>
        /// Gets or sets the subject profile picture.
        /// </summary>
        [Description("picture")]
        public string? Picture { get; init; }

        /// <summary>
        /// Gets or sets the subject locale.
        /// </summary>
        [Description("locale")]
        public string? Locale { get; init; }

        #endregion
    }
}
