﻿using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Nameless.Logging;
using StackExchange.Redis;

namespace Nameless.Caching.Redis {
    public sealed class RedisDatabaseManager : IRedisDatabaseManager, IDisposable {

        #region Private Read-Only Fields

        private readonly RedisOptions _options;

        #endregion

        #region Public Properties

        private ILogger? _logger;
        public ILogger Logger {
            get { return _logger ??= NullLogger.Instance; }
            set { _logger = value; }
        }

        #endregion

        #region Private Fields

        private IConnectionMultiplexer _multiplexer = default!;
        private IDatabase _database = default!;
        private bool _disposed;

        #endregion

        #region Public Constructors

        public RedisDatabaseManager(RedisOptions options) {
            _options = options ?? RedisOptions.Default;

            Initialize();
        }

        #endregion

        #region Destructor

        ~RedisDatabaseManager() => Dispose(disposing: false);

        #endregion

        #region Private Methods

        private void Initialize() {
            var opts = new ConfigurationOptions {
                EndPoints = { { _options.Host, _options.Port } },
                User = _options.Username,
                Password = _options.Password,
                KeepAlive = _options.KeepAlive,
            };

            if (_options.Ssl != default && _options.Ssl.Host != default) {
                opts.EndPoints.Add(_options.Ssl.Host, _options.Ssl.Port);
            }

            if (_options.Certificate != default) {
                opts.CertificateSelection += OnCertificateSelection;
                opts.CertificateValidation += OnCertificateValidation;
            }

            _multiplexer = ConnectionMultiplexer.Connect(opts);
            _database = _multiplexer.GetDatabase();
        }

        private X509Certificate OnCertificateSelection(object sender, string targetHost, X509CertificateCollection localCertificates, X509Certificate? remoteCertificate, string[] acceptableIssuers) {
            if (_options.Certificate == default || _options.Certificate.Pfx == default) {
                throw new InvalidOperationException("The path to the .pfx file was not specified.");
            }

            return new X509Certificate2(_options.Certificate.Pfx, _options.Certificate.Pass);
        }

        private bool OnCertificateValidation(object sender, X509Certificate? certificate, X509Chain? chain, SslPolicyErrors sslPolicyErrors) {
            if (_options.Certificate == default || _options.Certificate.Pem == default) {
                throw new InvalidOperationException("The path to the .pem file was not specified.");
            }

            if (certificate == default) { return false; }

            var inner = new X509Certificate2(_options.Certificate.Pem);
            var verdict = certificate.Issuer == inner.Subject;

            Logger.On(verdict).Information("Certificate error: {0}", sslPolicyErrors);

            return verdict;
        }

        private void BlockAccessAfterDispose() => ObjectDisposedException.ThrowIf(_disposed, this);

        private void Dispose(bool disposing) {
            if (_disposed) { return; }
            if (disposing) {
                _multiplexer.Dispose();
            }

            _multiplexer = default!;
            _database = default!;
            _disposed = true;
        }

        #endregion

        #region IRedisDatabaseManager Members

        public IDatabase GetDatabase() {
            BlockAccessAfterDispose();

            return _database;
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
