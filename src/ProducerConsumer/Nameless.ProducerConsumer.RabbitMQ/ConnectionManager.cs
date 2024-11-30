using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nameless.ProducerConsumer.RabbitMQ.Internals;
using Nameless.ProducerConsumer.RabbitMQ.Options;
using RabbitMQ.Client;

namespace Nameless.ProducerConsumer.RabbitMQ;

public sealed class ConnectionManager : IConnectionManager, IDisposable, IAsyncDisposable {
    private readonly ILogger<ConnectionManager> _logger;
    private readonly IOptions<RabbitMQOptions> _options;

    private ConnectionFactory? _connectionFactory;
    private IConnection? _connection;
    private SemaphoreSlim? _semaphore;
    private bool _disposed;

    public ConnectionManager(ILogger<ConnectionManager> logger, IOptions<RabbitMQOptions> options) {
        _logger = Prevent.Argument.Null(logger);
        _options = Prevent.Argument.Null(options);
    }

    ~ConnectionManager() {
        Dispose(disposing: false);
    }

    /// <inheritdoc />
    public async Task<IConnection> GetConnectionAsync(CancellationToken cancellationToken) {
        await GetSemaphoreSlim().WaitAsync(cancellationToken);

        try { return _connection ??= await GetConnectionFactory().CreateConnectionAsync(cancellationToken)
                                                                 .ConfigureAwait(continueOnCapturedContext: false); }
        finally { GetSemaphoreSlim().Release(); }
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

    private void Dispose(bool disposing) {
        if (_disposed) { return; }

        if (disposing) {
            _connection?.Dispose();
            _semaphore?.Dispose();
        }

        _connectionFactory = null;
        _connection = null;
        _semaphore = null;

        _disposed = true;
    }

    private async ValueTask DisposeAsyncCore() {
        if (_connection is not null) {
            await _connection.CloseAsync(global::RabbitMQ.Client.Constants.ReplySuccess, "Disposing RabbitMQ connection.")
                             .ConfigureAwait(continueOnCapturedContext: false);
            await _connection.DisposeAsync()
                             .ConfigureAwait(continueOnCapturedContext: false);
        }

        _semaphore?.Dispose();
    }

    private SemaphoreSlim GetSemaphoreSlim()
        => _semaphore ??= new SemaphoreSlim(initialCount: 1, maxCount: 1);

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
            _connectionFactory.Ssl.ServerName = Prevent.Argument.Null(opts.Server.Ssl.ServerName);
            _connectionFactory.Ssl.Version = opts.Server.Ssl.Protocol;
            _connectionFactory.Ssl.AcceptablePolicyErrors = opts.Server.Ssl.PolicyError;
        }

        if (opts.Server.Certificate.IsAvailable) {
            SetCertificateSelectionCallback(_connectionFactory, opts);
            SetCertificateValidationCallback(_connectionFactory, _logger, opts);
        }

        return _connectionFactory;
    }

    private static void SetCertificateValidationCallback(ConnectionFactory connectionFactory, ILogger<ConnectionManager> logger, RabbitMQOptions opts) {
        connectionFactory.Ssl.CertificateValidationCallback = (_, certificate, _, sslPolicyErrors) => {
            if (certificate is null) { return false; }

            var pemPath = Prevent.Argument.Null(opts.Server.Certificate.Pem);

#if NET9_0
            var inner = X509CertificateLoader.LoadCertificateFromFile(pemPath);
#else
            var inner = new X509Certificate2(pemPath);
#endif

            var verdict = certificate.Issuer == inner.Subject;

            if (verdict) {
                logger.ErrorOnCertificateValidation(sslPolicyErrors);
            }

            return verdict;
        };
    }

    private static void SetCertificateSelectionCallback(ConnectionFactory connectionFactory, RabbitMQOptions opts) {
        var pfxPath = Prevent.Argument.Null(opts.Server.Certificate.Pfx);

        connectionFactory.Ssl.CertificateSelectionCallback = (_, _, _, _, _) => {
#if NET9_0
            return X509CertificateLoader.LoadPkcs12FromFile(path: pfxPath,
                                                            password: opts.Server.Certificate.Password);
#else
            return new X509Certificate(fileName: pfxPath,
                                       password: opts.Server.Certificate.Password);
#endif
        };
    }
}