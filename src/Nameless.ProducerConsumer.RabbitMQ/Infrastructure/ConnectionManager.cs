using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nameless.ProducerConsumer.RabbitMQ.Internals;
using Nameless.ProducerConsumer.RabbitMQ.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace Nameless.ProducerConsumer.RabbitMQ.Infrastructure;

/// <summary>
///     Default implementation of <see cref="IConnectionManager" /> for managing RabbitMQ connections.
/// </summary>
public sealed class ConnectionManager : IConnectionManager, IDisposable, IAsyncDisposable {
    private readonly IOptions<RabbitMQOptions> _options;
    private readonly ILogger<ConnectionManager> _logger;

    private ConnectionFactory? _connectionFactory;
    private IConnection? _connection;
    private bool _disposed;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ConnectionManager" /> class.
    /// </summary>
    /// <param name="options">The RabbitMQ options.</param>
    /// <param name="logger">The logger.</param>
    public ConnectionManager(IOptions<RabbitMQOptions> options, ILogger<ConnectionManager> logger) {
        _options = Throws.When.Null(options);
        _logger = Throws.When.Null(logger);
    }

    /// <inheritdoc />
    ~ConnectionManager() {
        Dispose(disposing: false);
    }

    /// <inheritdoc />
    /// <exception cref="BrokerUnreachableException">
    ///     When the RabbitMQ broker is unreachable or the connection cannot be established.
    /// </exception>
    public async Task<IConnection> GetConnectionAsync(CancellationToken cancellationToken) {
        BlockAccessAfterDispose();

        try {
            return _connection ??= await GetConnectionFactory().CreateConnectionAsync(cancellationToken)
                                                               .SkipContextSync();
        }
        catch (BrokerUnreachableException ex) {
            _logger.BrokerUnreachable(_options.Value.Server, ex);

            throw;
        }
    }

    /// <inheritdoc />
    public void Dispose() {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync() {
        await DisposeAsyncCore().ConfigureAwait(continueOnCapturedContext: false);

        Dispose(disposing: false);
        GC.SuppressFinalize(this);
    }

    private void BlockAccessAfterDispose() {
        ObjectDisposedException.ThrowIf(_disposed, this);
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

    private async ValueTask DisposeAsyncCore() {
        if (_connection is not null) {
            await _connection.CloseAsync(Constants.ReplySuccess, reasonText: "Disposing RabbitMQ connection.")
                             .SkipContextSync();

            await _connection.DisposeAsync()
                             .ConfigureAwait(continueOnCapturedContext: false);
        }
    }

    private ConnectionFactory GetConnectionFactory() {
        if (_connectionFactory is not null) {
            return _connectionFactory;
        }

        var opts = _options.Value;

        _connectionFactory = new ConnectionFactory {
            HostName = opts.Server.Hostname, Port = opts.Server.Port, VirtualHost = opts.Server.VirtualHost
        };

        SetCredentials(_connectionFactory, opts);
        SetSslAuthentication(_connectionFactory, opts);
        SetCertificateSelectionCallback(_connectionFactory, opts);

        return _connectionFactory;
    }

    private static void SetCredentials(ConnectionFactory connectionFactory, RabbitMQOptions opts) {
        if (!opts.Server.UseCredentials) { return; }

        connectionFactory.UserName = opts.Server.Username;
        connectionFactory.Password = opts.Server.Password;
    }

    private static void SetSslAuthentication(ConnectionFactory connectionFactory, RabbitMQOptions opts) {
        if (!opts.Server.Ssl.IsAvailable) { return; }

        connectionFactory.Ssl.Enabled = true;
        connectionFactory.Ssl.ServerName = opts.Server.Ssl.ServerName;
        connectionFactory.Ssl.Version = opts.Server.Ssl.Protocol;
        connectionFactory.Ssl.AcceptablePolicyErrors = opts.Server.Ssl.AcceptablePolicyErrors;
    }

    private static void SetCertificateSelectionCallback(ConnectionFactory connectionFactory, RabbitMQOptions opts) {
        if (!opts.Server.Certificate.IsAvailable) { return; }

        connectionFactory.Ssl.CertificateSelectionCallback = (_, _, _, _, _)
            => X509CertificateLoader.LoadPkcs12FromFile(opts.Server.Certificate.CertPath,
                opts.Server.Certificate.CertPassword);
    }
}