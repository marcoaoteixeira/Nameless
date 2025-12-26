using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Nameless.Microservices.App.Internals;
using Nameless.ObjectModel;
using Nameless.Results;

namespace Nameless.Microservices.App.Infrastructure.Sqlite;

public class SqliteHealthCheck : IHealthCheck {
    private readonly IConfiguration _configuration;
    private readonly ILogger<SqliteHealthCheck> _logger;

    public SqliteHealthCheck(IConfiguration configuration, ILogger<SqliteHealthCheck> logger) {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default) {
        var dbConnResult = GetDbConnection();
        if (!dbConnResult.Success) {
            return HealthCheckResult.Unhealthy(dbConnResult.Errors[0].Message);
        }

        await using var connection = dbConnResult.Value;

        var checkConnectionResult = await CheckDatabaseConnectionAsync(connection, cancellationToken);
        if (!checkConnectionResult.Success) {
            return HealthCheckResult.Unhealthy(checkConnectionResult.Errors[0].Message);
        }

        var executeQueryResult = await ExecuteDatabaseQueryAsync(connection, cancellationToken);
        if (!executeQueryResult.Success) {
            return HealthCheckResult.Unhealthy(executeQueryResult.Errors[0].Message);
        }

        return HealthCheckResult.Healthy("Sqlite is working properly.");
    }

    private Result<DbConnection> GetDbConnection() {
        var connStr = _configuration.GetConnectionString(Constants.Database.CONN_STR_NAME);

        if (!string.IsNullOrWhiteSpace(connStr)) {
            return new SqliteConnection(connStr);
        }

        _logger.SqliteMissingConnString(Constants.Database.CONN_STR_NAME);

        return Error.Missing("Sqlite is not configured.");

    }

    private async Task<Result<bool>> CheckDatabaseConnectionAsync(DbConnection connection, CancellationToken cancellationToken) {
        try { await connection.OpenAsync(cancellationToken).SkipContextSync(); }
        catch (Exception ex) {
            _logger.SqliteConnectionFailed(ex);

            return Error.Failure("Unable to connect to Sqlite.");
        }

        return true;
    }

    private async Task<Result<bool>> ExecuteDatabaseQueryAsync(DbConnection connection, CancellationToken cancellationToken) {
        try {
            await using var command = connection.CreateCommand();

            command.CommandText = "SELECT 1 FROM todos";

            _ = await command.ExecuteReaderAsync(cancellationToken).SkipContextSync();
        }
        catch (Exception ex) {
            _logger.SqliteQueryFailed(ex);

            return Error.Failure("Unable to query Sqlite database.");
        }

        return true;
    }
}
