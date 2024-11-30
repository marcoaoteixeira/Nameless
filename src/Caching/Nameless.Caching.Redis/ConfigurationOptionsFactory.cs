using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nameless.Caching.Redis.Internals;
using Nameless.Caching.Redis.Options;
using StackExchange.Redis;

namespace Nameless.Caching.Redis;

/// <summary>
/// Implementation of <see cref="IConfigurationOptionsFactory"/>.
/// </summary>
public sealed class ConfigurationOptionsFactory : IConfigurationOptionsFactory {
    private readonly ILogger<ConfigurationOptionsFactory> _logger;
    private readonly IOptions<RedisOptions> _options;

    /// <summary>
    /// Initializes a new instance of <see cref="ConfigurationOptionsFactory"/>
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="options">Redis options.</param>
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="options"/> or
    /// <paramref name="logger"/> is <c>null</c>.
    /// </exception>
    public ConfigurationOptionsFactory(ILogger<ConfigurationOptionsFactory> logger, IOptions<RedisOptions> options) {
        _logger = Prevent.Argument.Null(logger);
        _options = Prevent.Argument.Null(options);
    }

    /// <inheritdoc />
    public ConfigurationOptions CreateConfigurationOptions() {
        var opts = _options.Value;

        var (host, port, protocol) = opts.Ssl.IsAvailable
            ? (opts.Ssl.Host, opts.Ssl.Port, opts.Ssl.Protocol)
            : (opts.Host, opts.Port, SslProtocols.None);

        var result = new ConfigurationOptions {
            EndPoints = { { host, port } },
            User = opts.Username,
            Password = opts.Password,
            KeepAlive = opts.KeepAlive,
            Ssl = opts.Ssl.IsAvailable,
            SslProtocols = protocol
        };

        if (opts.Certificate.IsAvailable) {
            result.CertificateSelection += OnCertificateSelection;
            result.CertificateValidation += OnCertificateValidation;
        }

        return result;
    }

    private X509Certificate2 OnCertificateSelection(
        object sender,
        string targetHost,
        X509CertificateCollection localCertificates,
        X509Certificate? remoteCertificate,
        string[] acceptableIssuers) {
#if NET9_0
        return X509CertificateLoader.LoadPkcs12FromFile(path: _options.Value.Certificate.Pfx,
                                                        password: _options.Value.Certificate.Password);
#else
        return new X509Certificate2(fileName: _options.Value.Certificate.Pfx,
                                   password: _options.Value.Certificate.Password);
#endif
    }

    private bool OnCertificateValidation(
        object sender,
        X509Certificate? certificate,
        X509Chain? chain,
        SslPolicyErrors sslPolicyErrors) {
        if (certificate is null) { return false; }

#if NET9_0
        var inner = X509CertificateLoader.LoadCertificateFromFile(_options.Value.Certificate.Pem);
#else
        var inner = new X509Certificate2(_options.Value.Certificate.Pem);
#endif

        var verdict = certificate.Issuer == inner.Subject;

        _logger.OnCondition(verdict)
               .CertificateValidationError(sslPolicyErrors);

        return verdict;
    }
}