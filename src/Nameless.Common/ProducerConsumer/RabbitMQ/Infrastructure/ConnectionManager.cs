using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nameless.ProducerConsumer.RabbitMQ.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace Nameless.ProducerConsumer.RabbitMQ.Infrastructure;

/// <summary>
///     Default implementation of <see cref="IConnectionManager" /> for managing RabbitMQ connections.
/// </summary>
public sealed class ConnectionManager : IConnectionManager, IDisposable, IAsyncDisposable {
    private readonly ServerOptions _server;
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
        _server = options.Value.Server;
        _logger = logger;
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
            Log.BrokerUnreachable(_logger, _server.Hostname, ex);

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
            await _connection.CloseAsync(RabbitConstants.ReplySuccess, reasonText: "Disposing RabbitMQ connection.")
                             .SkipContextSync();

            await _connection.DisposeAsync()
                             .ConfigureAwait(continueOnCapturedContext: false);
        }
    }

    private ConnectionFactory GetConnectionFactory() {
        if (_connectionFactory is not null) {
            return _connectionFactory;
        }

        _connectionFactory = new ConnectionFactory {
            HostName = _server.Hostname,
            Port = _server.Port,
            VirtualHost = _server.VirtualHost
        };

        SetCredentials(_connectionFactory, _server);
        SetSslAuthentication(_connectionFactory, _server);
        SetCertificateSelectionCallback(_connectionFactory, _server);

        return _connectionFactory;
    }

    private static void SetCredentials(ConnectionFactory connectionFactory, ServerOptions opts) {
        if (!opts.UseCredentials) { return; }

        connectionFactory.UserName = opts.Username;
        connectionFactory.Password = opts.Password;
    }

    private static void SetSslAuthentication(ConnectionFactory connectionFactory, ServerOptions opts) {
        if (opts.Ssl is null || !opts.Ssl.IsAvailable) { return; }

        connectionFactory.Ssl.Enabled = true;
        connectionFactory.Ssl.ServerName = opts.Ssl.ServerName;
        connectionFactory.Ssl.Version = opts.Ssl.Protocol;
        connectionFactory.Ssl.AcceptablePolicyErrors = opts.Ssl.AcceptablePolicyErrors;
    }

    private static void SetCertificateSelectionCallback(ConnectionFactory connectionFactory, ServerOptions opts) {
        if (opts.Certificate is null || !opts.Certificate.IsAvailable) { return; }

        connectionFactory.Ssl.CertificateSelectionCallback = (_, _, _, _, _)
            => X509CertificateLoader.LoadPkcs12FromFile(
                opts.Certificate.CertPath,
                opts.Certificate.CertPassword
            );
    }
}