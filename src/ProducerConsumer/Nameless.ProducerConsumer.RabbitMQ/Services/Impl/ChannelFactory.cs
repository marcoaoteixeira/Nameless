using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Logging;
using Nameless.ProducerConsumer.RabbitMQ.Internals;
using Nameless.ProducerConsumer.RabbitMQ.Options;
using RabbitMQ.Client;

namespace Nameless.ProducerConsumer.RabbitMQ.Services.Impl;

public sealed class ChannelFactory : IChannelFactory, IDisposable {
    private readonly RabbitMQOptions _options;
    private readonly ILogger _logger;

    private ConnectionFactory? _connectionFactory;
    private IConnection? _connection;
    private bool _disposed;

    public ChannelFactory(RabbitMQOptions options, ILogger<ChannelFactory> logger) {
        _options = Prevent.Argument.Null(options);
        _logger = Prevent.Argument.Null(logger);
    }

    ~ChannelFactory() {
        Dispose(disposing: false);
    }

    public IModel CreateChannel() {
        BlockAccessAfterDispose();

        return GetConnection().CreateModel();
    }

    public void Dispose() {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

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
        if (_connectionFactory is not null) {
            return _connectionFactory;
        }

        _connectionFactory = new ConnectionFactory {
            HostName = _options.Server.Hostname,
            Port = _options.Server.Port,
            VirtualHost = _options.Server.VirtualHost,

            // We're using AsyncEventingBasicConsumer
            // so, we must set this to true
            // https://stackoverflow.com/questions/47847590/explain-asynceventingbasicconsumer-behaviour-without-dispatchconsumersasync-tr
            DispatchConsumersAsync = true
        };

        if (_options.Server.UseCredentials) {
            _connectionFactory.UserName = _options.Server.Username;
            _connectionFactory.Password = _options.Server.Password;
        }

        if (_options.Server.Ssl.IsAvailable) {
            _connectionFactory.Ssl = new SslOption(_options.Server.Ssl.ServerName, enabled: _options.Server.Ssl.Enabled) {
                Version = _options.Server.Ssl.Protocol,
                AcceptablePolicyErrors = _options.Server.Ssl.PolicyError
            };
        }

        if (_options.Server.Certificate.IsAvailable && _connectionFactory.Ssl is not null) {
            _connectionFactory.Ssl.CertificateSelectionCallback = (_, _, _, _, _)
                => new X509Certificate(fileName: _options.Server.Certificate.Pfx,
                                       password: _options.Server.Certificate.Password);

            _connectionFactory.Ssl.CertificateValidationCallback = (_, certificate, _, sslPolicyErrors) => {
                if (certificate is null) {
                    return false;
                }

                var inner = new X509Certificate2(_options.Server.Certificate.Pem);
                var verdict = certificate.Issuer == inner.Subject;

                if (verdict) {
                    LoggerHandlers.CertificateValidationFailure(_logger, sslPolicyErrors, null /* exception */);
                }

                return verdict;
            };
        }

        return _connectionFactory;
    }

    private IConnection GetConnection()
        => _connection ??= GetConnectionFactory().CreateConnection();
}