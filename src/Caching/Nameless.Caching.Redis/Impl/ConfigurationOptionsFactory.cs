using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Nameless.Caching.Redis.Options;
using Nameless.Logging.Microsoft;
using StackExchange.Redis;

namespace Nameless.Caching.Redis.Impl {
    public sealed class ConfigurationOptionsFactory : IConfigurationOptionsFactory {
        #region Private Read-Only Fields

        private readonly RedisOptions _options;
        private readonly ILogger _logger;

        #endregion

        #region Public Constructors

        public ConfigurationOptionsFactory()
            : this(RedisOptions.Default, NullLogger.Instance) { }

        public ConfigurationOptionsFactory(RedisOptions options, ILogger logger) {
            _options = Guard.Against.Null(options, nameof(options));
            _logger = Guard.Against.Null(logger, nameof(logger));
        }

        #endregion

        #region Private Methods

        private X509Certificate OnCertificateSelection(object sender, string targetHost, X509CertificateCollection localCertificates, X509Certificate? remoteCertificate, string[] acceptableIssuers)
            => new(
                fileName: _options.Certificate.Pfx,
                password: _options.Certificate.Password
            );

        private bool OnCertificateValidation(object sender, X509Certificate? certificate, X509Chain? chain, SslPolicyErrors sslPolicyErrors) {
            if (certificate is null) {
                return false;
            }

            var inner = new X509Certificate2(_options.Certificate.Pem);
            var verdict = certificate.Issuer == inner.Subject;

            _logger
                .On(verdict)
                .LogInformation(
                    message: "Certificate error: {sslPolicyErrors}",
                    args: sslPolicyErrors
                );

            return verdict;
        }

        #endregion

        #region IConfigurationOptionsFactory Members

        public ConfigurationOptions CreateConfigurationOptions() {
            var (host, port, protocol) = _options.Ssl.IsAvailable()
                ? (_options.Ssl.Host, _options.Ssl.Port, _options.Ssl.Protocol)
                : (_options.Host, _options.Port, SslProtocols.None);

            var result = new ConfigurationOptions {
                EndPoints = { { host, port } },
                User = _options.Username,
                Password = _options.Password,
                KeepAlive = _options.KeepAlive,
                Ssl = _options.Ssl.IsAvailable(),
                SslProtocols = protocol
            };

            if (_options.Certificate.IsAvailable()) {
                result.CertificateSelection += OnCertificateSelection;
                result.CertificateValidation += OnCertificateValidation;
            }

            return result;
        }

        #endregion
    }
}
