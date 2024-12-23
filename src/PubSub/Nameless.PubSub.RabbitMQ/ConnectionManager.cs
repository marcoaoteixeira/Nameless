using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Options;
using Nameless.PubSub.RabbitMQ.Contracts;
using Nameless.PubSub.RabbitMQ.Options;
using RabbitMQ.Client;

namespace Nameless.PubSub.RabbitMQ;

public sealed class ConnectionManager : IConnectionManager, IDisposable, IAsyncDisposable {
    private readonly IOptions<RabbitMQOptions> _options;

    private ConnectionFactory? _connectionFactory;
    private IConnection? _connection;
    private bool _disposed;

    public ConnectionManager(IOptions<RabbitMQOptions> options) {
        _options = Prevent.Argument.Null(options);
    }

    ~ConnectionManager() {
        Dispose(disposing: false);
    }

    /// <inheritdoc />
    public async Task<IConnection> GetConnectionAsync(CancellationToken cancellationToken) {
        BlockAccessAfterDispose();

        return _connection ??= await GetConnectionFactory().CreateConnectionAsync(cancellationToken)
                                                           .ConfigureAwait(continueOnCapturedContext: false);
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
#if NET8_0_OR_GREATER
        ObjectDisposedException.ThrowIf(_disposed, this);
#else
        if (_disposed) {
            throw new ObjectDisposedException(nameof(ConnectionManager));
        }
#endif
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
            await _connection.CloseAsync(Constants.ReplySuccess, "Disposing RabbitMQ connection.")
                             .ConfigureAwait(continueOnCapturedContext: false);
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
            HostName = opts.Server.Hostname,
            Port = opts.Server.Port,
            VirtualHost = opts.Server.VirtualHost
        };

        if (opts.Server.UseCredentials) {
            _connectionFactory.UserName = opts.Server.Username;
            _connectionFactory.Password = opts.Server.Password;
        }

        if (opts.Server.Ssl.IsAvailable) {
            _connectionFactory.Ssl.ServerName = opts.Server.Ssl.ServerName;
            _connectionFactory.Ssl.Version = opts.Server.Ssl.Protocol;
            _connectionFactory.Ssl.AcceptablePolicyErrors = opts.Server.Ssl.AcceptablePolicyErrors;
        }

        if (opts.Server.Certificate.IsAvailable) {
            SetCertificateSelectionCallback(_connectionFactory, opts);
        }

        return _connectionFactory;
    }

    private static void SetCertificateSelectionCallback(ConnectionFactory connectionFactory, RabbitMQOptions opts) {
        var certPath = Prevent.Argument.Null(opts.Server.Certificate.CertPath);

        connectionFactory.Ssl.CertificateSelectionCallback = (_, _, _, _, _) => {
#if NET9_0_OR_GREATER
            return X509CertificateLoader.LoadPkcs12FromFile(path: certPath,
                                                            password: opts.Server.Certificate.CertPassword);
#else
            return new X509Certificate(fileName: certPath,
                                       password: opts.Server.Certificate.CertPassword);
#endif
        };
    }
}