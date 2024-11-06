using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nameless.ProducerConsumer.RabbitMQ.Internals;
using Nameless.ProducerConsumer.RabbitMQ.Options;
using RabbitMQ.Client;

namespace Nameless.ProducerConsumer.RabbitMQ;

public sealed class ChannelManager : IChannelManager, IDisposable {
    private static readonly object SyncLock = new();

    private readonly IOptions<RabbitMQOptions> _options;
    private readonly ILogger<ChannelManager> _logger;

    private ConnectionFactory? _connectionFactory;
    private IConnection? _connection;
    private IModel? _channel;
    private bool _disposed;

    public ChannelManager(IOptions<RabbitMQOptions> options, ILogger<ChannelManager> logger) {
        _options = Prevent.Argument.Null(options);
        _logger = Prevent.Argument.Null(logger);
    }

    ~ChannelManager() {
        Dispose(disposing: false);
    }

    public IModel GetChannel() {
        BlockAccessAfterDispose();

        lock (SyncLock) {
            if (_channel is not (null or { IsClosed: true })) {
                return _channel;
            }

            DestroyChannel(ref _channel);

            return _channel ??= GetConnection().CreateModel();
        }
    }

    public void Dispose() {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private static void DestroyChannel(ref IModel? channel) {
        if (channel is null) { return; }

        channel.Dispose();
        channel = null;
    }

    private void BlockAccessAfterDispose() {
        if (_disposed) {
            throw new ObjectDisposedException(nameof(ChannelManager));
        }
    }

    private void Dispose(bool disposing) {
        if (_disposed) { return; }

        if (disposing) {
            _channel?.Close(global::RabbitMQ.Client.Constants.ReplySuccess, "Disposing channel manager.");
            _channel?.Dispose();

            _connection?.Close(global::RabbitMQ.Client.Constants.ReplySuccess, "Disposing channel manager.");
            _connection?.Dispose();
        }

        _channel = null;
        _connection = null;
        _connectionFactory = null;

        _disposed = true;
    }

    private ConnectionFactory GetConnectionFactory() {
        if (_connectionFactory is not null) {
            return _connectionFactory;
        }

        var opts = _options.Value;

        _connectionFactory = new ConnectionFactory {
            HostName = opts.Server.Hostname,
            Port = opts.Server.Port,
            VirtualHost = opts.Server.VirtualHost,

            // We're using AsyncEventingBasicConsumer
            // so, we must set this to true
            // https://stackoverflow.com/questions/47847590/explain-asynceventingbasicconsumer-behaviour-without-dispatchconsumersasync-tr
            DispatchConsumersAsync = true
        };

        if (opts.Server.UseCredentials) {
            _connectionFactory.UserName = opts.Server.Username;
            _connectionFactory.Password = opts.Server.Password;
        }

        if (opts.Server.Ssl.IsAvailable) {
            _connectionFactory.Ssl = new SslOption(opts.Server.Ssl.ServerName, enabled: opts.Server.Ssl.Enabled) {
                Version = opts.Server.Ssl.Protocol,
                AcceptablePolicyErrors = opts.Server.Ssl.PolicyError
            };
        }

        if (opts.Server.Certificate.IsAvailable && _connectionFactory.Ssl is not null) {
            _connectionFactory.Ssl.CertificateSelectionCallback = (_, _, _, _, _)
                => new X509Certificate(fileName: opts.Server.Certificate.Pfx,
                                       password: opts.Server.Certificate.Password);

            _connectionFactory.Ssl.CertificateValidationCallback = (_, certificate, _, sslPolicyErrors) => {
                if (certificate is null) {
                    return false;
                }

                var inner = new X509Certificate2(opts.Server.Certificate.Pem);
                var verdict = certificate.Issuer == inner.Subject;

                if (verdict) {
                    _logger.ErrorOnCertificateValidation(sslPolicyErrors);
                }

                return verdict;
            };
        }

        return _connectionFactory;
    }

    private IConnection GetConnection()
        => _connection ??= GetConnectionFactory().CreateConnection();
}