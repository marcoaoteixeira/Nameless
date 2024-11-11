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
    private readonly IOptions<RedisOptions> _options;
    private readonly ILogger<ConfigurationOptionsFactory> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="ConfigurationOptionsFactory"/>
    /// </summary>
    /// <param name="options">Redis options.</param>
    /// <param name="logger">The logger instance.</param>
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="options"/> or
    /// <paramref name="logger"/> is <c>null</c>.
    /// </exception>
    public ConfigurationOptionsFactory(IOptions<RedisOptions> options, ILogger<ConfigurationOptionsFactory> logger) {
        _options = Prevent.Argument.Null(options);
        _logger = Prevent.Argument.Null(logger);
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

    private X509Certificate OnCertificateSelection(
        object sender,
        string targetHost,
        X509CertificateCollection localCertificates,
        X509Certificate? remoteCertificate,
        string[] acceptableIssuers)
        => new(fileName: _options.Value.Certificate.Pfx,
               password: _options.Value.Certificate.Password);

    private bool OnCertificateValidation(
        object sender,
        X509Certificate? certificate,
        X509Chain? chain,
        SslPolicyErrors sslPolicyErrors) {
        if (certificate is null) { return false; }

        var inner = new X509Certificate2(_options.Value.Certificate.Pem);
        var verdict = certificate.Issuer == inner.Subject;

        _logger.OnCondition(verdict)
               .CertificateValidationError(sslPolicyErrors);

        return verdict;
    }
}