using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Nameless.Logging.Microsoft;
using Nameless.ProducerConsumer.RabbitMQ.Options;
using RabbitMQ.Client;

namespace Nameless.ProducerConsumer.RabbitMQ.Services.Impl {
    public sealed class ChannelFactory : IChannelFactory, IDisposable {
        #region Private Read-Only Fields

        private readonly RabbitMQOptions _options;
        private readonly ILogger _logger;

        #endregion

        #region Private Fields

        private ConnectionFactory? _connectionFactory;
        private IConnection? _connection;
        private bool _disposed;

        #endregion

        #region Public Constructors

        public ChannelFactory()
            : this(RabbitMQOptions.Default, NullLogger.Instance) { }

        public ChannelFactory(RabbitMQOptions options, ILogger logger) {
            _options = Guard.Against.Null(options, nameof(options));
            _logger = Guard.Against.Null(logger, nameof(logger));
        }

        #endregion

        #region Private Methods

        private void BlockAccessAfterDispose() {
            if (_disposed) {
                throw new ObjectDisposedException(nameof(ChannelFactory));
            }
        }

        private void Dispose(bool disposing) {
            if (_disposed) { return; }
            if (disposing) {
                _connection?.Dispose();
            }

            _connectionFactory = null;
            _connection = null;
            _disposed = true;
        }

        private ConnectionFactory GetConnectionFactory() {
            if (_connectionFactory is null) {
                _connectionFactory = new ConnectionFactory {
                    HostName = _options.Server.Hostname,
                    Port = _options.Server.Port,
                    VirtualHost = _options.Server.VirtualHost,

                    // We're using AsyncEventingBasicConsumer
                    // so, we must set this to true
                    // https://stackoverflow.com/questions/47847590/explain-asynceventingbasicconsumer-behaviour-without-dispatchconsumersasync-tr
                    DispatchConsumersAsync = true
                };

                if (_options.Server.UseCredentials()) {
                    _connectionFactory.UserName = _options.Server.Username;
                    _connectionFactory.Password = _options.Server.Password;
                }

                if (_options.Server.Ssl.IsAvailable()) {
                    _connectionFactory.Ssl = new(_options.Server.Ssl.ServerName, enabled: _options.Server.Ssl.Enabled) {
                        Version = _options.Server.Ssl.Protocol,
                        AcceptablePolicyErrors = _options.Server.Ssl.PolicyError,
                        CertificateSelectionCallback = CertificateSelectionHandler,
                        CertificateValidationCallback = CertificateValidationHandler
                    };
                }
            }

            return _connectionFactory;
        }

        private X509Certificate CertificateSelectionHandler(object sender, string targetHost, X509CertificateCollection localCertificates, X509Certificate? remoteCertificate, string[] acceptableIssuers)
            => new(
                fileName: _options.Server.Certificate.Pfx!,
                password: _options.Server.Certificate.Pass
            );

        private bool CertificateValidationHandler(object sender, X509Certificate? certificate, X509Chain? chain, SslPolicyErrors sslPolicyErrors) {
            if (certificate is null) {
                return false;
            }

            var inner = new X509Certificate2(_options.Server.Certificate.Pem!);
            var verdict = certificate.Issuer == inner.Subject;

            _logger
                .On(verdict)
                .LogInformation(
                    message: "Certificate error: {sslPolicyErrors}",
                    args: sslPolicyErrors
                );

            return verdict;
        }

        private IConnection GetConnection()
            => _connection ??= GetConnectionFactory().CreateConnection();

        #endregion

        #region IChannelFactory Members

        public IModel CreateChannel() {
            BlockAccessAfterDispose();

            return GetConnection().CreateModel();
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
