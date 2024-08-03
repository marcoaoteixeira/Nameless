#if NET6_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif
using System.Security.Authentication;

namespace Nameless.Caching.Redis.Options {
    /// <summary>
    /// Provides properties to configure Redis SSL.
    /// </summary>
    public sealed record SslOptions {
        #region Public Static Read-Only Properties

        public static SslOptions Default => new();

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the SSL host.
        /// </summary>
        public string Host { get; set; } = "localhost";
        
        /// <summary>
        /// Gets or sets the SSL port.
        /// </summary>
        public int Port { get; set; }
        
        /// <summary>
        /// Gets or sets the SSL protocol.
        /// </summary>
        public SslProtocols Protocol { get; set; }

        /// <summary>
        /// Gets whether SSL configuration is available.
        /// </summary>
#if NET6_0_OR_GREATER
        [MemberNotNullWhen(returnValue: true, nameof(Host))]
#endif
        public bool IsAvailable
            => !string.IsNullOrWhiteSpace(Host) &&
               Port > 0 &&
               Protocol != SslProtocols.None;

        #endregion
    }
}
