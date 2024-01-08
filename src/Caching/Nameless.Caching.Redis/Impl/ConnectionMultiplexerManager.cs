using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Nameless.Logging.Microsoft;
using StackExchange.Redis;

namespace Nameless.Caching.Redis.Impl {
    public sealed class ConnectionMultiplexerManager : IConnectionMultiplexerManager, IDisposable {
        #region Private Read-Only Fields

        private readonly RedisOptions _options;
        private readonly ILogger _logger;

        #endregion

        #region Private Fields

        private ConnectionMultiplexer? _multiplexer;
        private ConfigurationOptions? _multiplexerConfigOpts;
        private bool _disposed;

        #endregion

        #region Public Constructors

        public ConnectionMultiplexerManager()
            : this(RedisOptions.Default, NullLogger.Instance) { }

        public ConnectionMultiplexerManager(RedisOptions options)
            : this(options, NullLogger.Instance) { }

        public ConnectionMultiplexerManager(RedisOptions options, ILogger logger) {
            _options = Guard.Against.Null(options, nameof(options));
            _logger = Guard.Against.Null(logger, nameof(logger));
        }

        #endregion

        #region Destructor

        ~ConnectionMultiplexerManager()
            => Dispose(disposing: false);

        #endregion

        #region Private Methods

        private void BlockAccessAfterDispose() {
            if (_disposed) {
                throw new ObjectDisposedException(nameof(ConnectionMultiplexerManager));
            }
        }

        private void Dispose(bool disposing) {
            if (_disposed) { return; }
            if (disposing) {
                if (_multiplexerConfigOpts is not null) {
                    _multiplexerConfigOpts.CertificateSelection -= CertificateSelectionHandler;
                    _multiplexerConfigOpts.CertificateValidation -= CertificateValidationHandler;
                }
                _multiplexer?.Dispose();
            }

            _multiplexer = null;
            _disposed = true;
        }

        private ConfigurationOptions CreateMultiplexerConfigurationOptions() {
            var host = _options.Ssl.Available
                ? _options.Ssl.Host ?? throw new ArgumentNullException("SSL Host")
                : _options.Host ?? throw new ArgumentNullException("Host");

            var port = _options.Ssl.Available
                ? _options.Ssl.Port
                : _options.Port;

            var sslProtocols = _options.Ssl.Available
                ? _options.Ssl.SslProtocol
                : SslProtocols.None;

            var opts = new ConfigurationOptions {
                EndPoints = { { host, port } },
                User = _options.Username,
                Password = _options.Password,
                KeepAlive = _options.KeepAlive,
                Ssl = _options.Ssl.Available,
                SslProtocols = sslProtocols,
            };

            if (_options.Certificate.Available) {
                opts.CertificateSelection += CertificateSelectionHandler;
                opts.CertificateValidation += CertificateValidationHandler;
            }

            return opts;
        }

        private X509Certificate2 CertificateSelectionHandler(object sender, string targetHost, X509CertificateCollection localCertificates, X509Certificate? remoteCertificate, string[] acceptableIssuers)
            => new(
                fileName: _options.Certificate.Pfx!,
                password: _options.Certificate.Pass
            );

        private bool CertificateValidationHandler(object sender, X509Certificate? certificate, X509Chain? chain, System.Net.Security.SslPolicyErrors sslPolicyErrors) {
            if (certificate is null) {
                return false;
            }

            var inner = new X509Certificate2(_options.Certificate.Pem!);
            var verdict = certificate.Issuer == inner.Subject;

            _logger
                .On(verdict)
                .LogInformation(
                    message: "Certificate error: {sslPolicyErrors}",
                    args: sslPolicyErrors
                );

            return verdict;
        }

        private ConnectionMultiplexer CreateMultiplexer() {
            _multiplexerConfigOpts = CreateMultiplexerConfigurationOptions();

            return ConnectionMultiplexer.Connect(_multiplexerConfigOpts);
        }

        #endregion

        #region RedisConnectionMultiplexerManager Members

        public IConnectionMultiplexer GetMultiplexer() {
            BlockAccessAfterDispose();

            return _multiplexer ??= CreateMultiplexer();
        }

        #endregion

        #region IDisposable Members

        public void Dispose() {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
