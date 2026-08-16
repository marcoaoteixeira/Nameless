using System.Data.Common;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Nameless.Testing.Tools.Helpers;
using Npgsql;
using Testcontainers.PostgreSql;
using Xunit;

namespace Nameless.Testing.Tools.Containers.Postgres;

/// <summary>
///     Defines a test collection for sharing a single instance of the
///     PostgresServer fixture across multiple test classes.
/// </summary>
/// <remarks>
///     Use this collection definition to group test classes that require
///     access to a shared PostgresServer instance. This approach ensures
///     consistent setup and teardown of the database server, improving
///     test reliability and performance. For more information on shared
///     context in xUnit, see https://xunit.net/docs/shared-context.
/// </remarks>
[CollectionDefinition(nameof(PostgresServerCollectionFixture))]
public class PostgresServerCollectionFixture : ICollectionFixture<PostgresServer>;

/// <summary>
///     Provides functionality to manage a Postgres test container instance
///     for testing purposes, including connection management and lifecycle
///     handling.
/// </summary>
public class PostgresServer : IAsyncLifetime
{
    private readonly PostgresServerOptions _options = ConfigurationHelper
        .CreateConfiguration()
        .GetOptions<PostgresServerOptions>()
        .Validate();

    private NpgsqlConnection? _connection;
    private PostgreSqlContainer? _container;
    private bool _disposed;

    /// <summary>
    ///     Retrieves a database connection to the current Postgres container instance.
    /// </summary>
    /// <returns>
    ///     An instance of <see cref="DbConnection"/>.
    /// </returns>
    /// <remarks>
    ///     This method will create a unique instance of <see cref="DbConnection"/>.
    ///     Multiple calls to <see cref="GetDbConnection"/> will return the same
    ///     instance.
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    ///     If the container is not running.
    /// </exception>
    public DbConnection GetDbConnection()
    {
        BlockAccessAfterDispose();

        if (_container is null || _container.State != TestcontainersStates.Running)
        {
            throw new InvalidOperationException("Postgres container is not available.");
        }

        if (_connection is not null)
        {
            return _connection;
        }

        var connectionString = _container.GetConnectionString();

        return _connection = new NpgsqlConnection(connectionString);
    }

    /// <inheritdoc />
    public async ValueTask InitializeAsync()
    {
        BlockAccessAfterDispose();

        var imageUrl = string.IsNullOrWhiteSpace(_options.RegistryUrl)
            ? _options.Image
            : string.Concat(_options.RegistryUrl, _options.Image);

        var builder = new PostgreSqlBuilder(imageUrl)
            .WithEnvironment(_options.Environment);

        if (_options.EnableSecurity)
        {
            builder
                .WithUsername(_options.Username)
                .WithPassword(_options.Password);
        }

        builder
            .WithDatabase(_options.Database)
            .WithPortBinding(_options.HostPort, _options.ContainerPort)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilInternalTcpPortIsAvailable(_options.ContainerPort));

        _container = builder.Build();

        await _container
            .StartAsync(TestContext.Current.CancellationToken)
            .ConfigureAwait(continueOnCapturedContext: false);
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (_disposed) { return; }

        await DisposeAsyncCore().ConfigureAwait(continueOnCapturedContext: false);
        GC.SuppressFinalize(this);

        _disposed = true;
    }

    private void BlockAccessAfterDispose()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
    }

    private async ValueTask DisposeAsyncCore()
    {
        if (_connection is not null)
        {
            await _connection.DisposeAsync().ConfigureAwait(continueOnCapturedContext: false);
        }

        if (_container is not null)
        {
            await _container.DisposeAsync().ConfigureAwait(continueOnCapturedContext: false);
        }

        _connection = null;
        _container = null;
    }
}
