﻿#if NET6_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif

namespace Nameless.Caching.Redis.Options {
    public sealed class CertificateOptions {
        #region Public Static Read-Only Properties

        public static CertificateOptions Default => new();

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the .pfx file path.
        /// </summary>
        public string Pfx { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the .pem file path.
        /// </summary>
        public string Pem { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the certificate password.
        /// </summary>
        public string? Password { get; set; }

#if NET6_0_OR_GREATER
        [MemberNotNullWhen(returnValue: true, nameof(Pfx), nameof(Pem), nameof(Password))]
#endif
        public bool IsAvailable
            => !string.IsNullOrWhiteSpace(Pfx) &&
               !string.IsNullOrWhiteSpace(Pem) &&
               !string.IsNullOrWhiteSpace(Password);

        #endregion
    }
}
