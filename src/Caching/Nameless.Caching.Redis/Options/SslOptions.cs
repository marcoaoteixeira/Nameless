#if NET6_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif
using System.Security.Authentication;

namespace Nameless.Caching.Redis.Options {
    public sealed class SslOptions {
        #region Public Static Read-Only Properties

        public static SslOptions Default => new();

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the SSL host.
        /// </summary>
        public string? Host { get; set; }
        /// <summary>
        /// Gets or sets the SSL port.
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// Gets or sets the SSL protocol.
        /// </summary>
        public SslProtocols SslProtocol { get; set; }

        #endregion

        #region Public Methods

#if NET6_0_OR_GREATER
        [MemberNotNullWhen(true, nameof(Host))]
#endif
        public bool IsAvailable()
            => !string.IsNullOrWhiteSpace(Host) &&
               Port > 0 &&
               SslProtocol != SslProtocols.None;

        #endregion
    }
}
