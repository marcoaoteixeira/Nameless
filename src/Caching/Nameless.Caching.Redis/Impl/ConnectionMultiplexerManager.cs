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

        #endregion

        #region Private Fields

        private IConnectionMultiplexer? _multiplexer;
        private bool _disposed;

        #endregion

        #region Public Properties

        private ILogger? _logger;
        public ILogger Logger {
            get => _logger ??= NullLogger.Instance;
            set => _logger = value;
        }

        #endregion

        #region Public Constructors

        public ConnectionMultiplexerManager(RedisOptions? options = null) {
            _options = options ?? RedisOptions.Default;
        }

        #endregion

        #region Destructor

        ~ConnectionMultiplexerManager()
            => Dispose(disposing: false);

        #endregion

        #region Private Methods

        private void BlockAccessAfterDispose() {
            if (_disposed) {
                throw new ObjectDisposedException(typeof(ConnectionMultiplexerManager).FullName);
            }
        }

        private void Dispose(bool disposing) {
            if (_disposed) { return; }
            if (disposing) {
                _multiplexer?.Dispose();
            }

            _multiplexer = null;
            _disposed = true;
        }

        private ConfigurationOptions GetConfigurationOptions() {
            var host = _options.Ssl.Available ? _options.Ssl.Host : _options.Host;
            var port = _options.Ssl.Available ? _options.Ssl.Port : _options.Port;
            var sslProtocols = _options.Ssl.Available ? _options.Ssl.SslProtocol : SslProtocols.None;

            var opts = new ConfigurationOptions {
                EndPoints = { { host, port } },
                User = _options.Username,
                Password = _options.Password,
                KeepAlive = _options.KeepAlive,
                Ssl = _options.Ssl.Available,
                SslProtocols = sslProtocols,
            };

            if (_options.Certificate.Available) {
                opts.CertificateSelection += (sender, targetHost, localCertificates, remoteCertificate, acceptableIssuers)
                    => new X509Certificate2(_options.Certificate.Pfx, _options.Certificate.Pass);

                opts.CertificateValidation += (sender, certificate, chain, sslPolicyErrors) => {
                    if (certificate is null) { return false; }

                    var inner = new X509Certificate2(_options.Certificate.Pem);
                    var verdict = certificate.Issuer == inner.Subject;

                    Logger.On(verdict).LogInformation("Certificate error: {sslPolicyErrors}", sslPolicyErrors);

                    return verdict;
                };
            }

            return opts;
        }

        private IConnectionMultiplexer CreateMultiplexer() {
            var opts = GetConfigurationOptions();

            return ConnectionMultiplexer.Connect(opts);
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
