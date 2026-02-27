using System.Data.Common;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nameless.ObjectModel;
using Nameless.Results;

namespace Nameless.Microservices.Infrastructure.HealthChecks.Database;

public class DatabaseHealthCheck : IHealthCheck {
    private readonly IDbConnectionFactory _dbConnectionFactory;
    private readonly IOptions<DatabaseHealthCheckOptions> _options;
    private readonly ILogger<DatabaseHealthCheck> _logger;
    
    protected DatabaseHealthCheck(
        IDbConnectionFactory dbConnectionFactory,
        IOptions<DatabaseHealthCheckOptions> options,
        ILogger<DatabaseHealthCheck> logger
    ) {
        _dbConnectionFactory = dbConnectionFactory;
        _options = options;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken()) {
        var createDbConnResult = CreateDbConnection();
        if (!createDbConnResult.Success) {
            return HealthCheckResult.Unhealthy(createDbConnResult.Errors[0].Message);
        }

        await using var connection = createDbConnResult.Value;

        var checkConnectionResult = await CheckConnectionAsync(connection, cancellationToken);
        if (!checkConnectionResult.Success) {
            return HealthCheckResult.Unhealthy(checkConnectionResult.Errors[0].Message);
        }

        var executeSqlResult = await ExecuteSqlAsync(connection, cancellationToken);
        if (!executeSqlResult.Success) {
            return HealthCheckResult.Unhealthy(executeSqlResult.Errors[0].Message);
        }

        return HealthCheckResult.Healthy("Database is working properly.");
    }

    private Result<DbConnection> CreateDbConnection() {
        try { return _dbConnectionFactory.CreateConnection(); }
        catch (Exception ex) { _logger.CreateDbConnectionFailure(ex); }

        return Error.Failure("Couldn't create database connection.");
    }

    private async Task<Result<bool>> CheckConnectionAsync(DbConnection connection, CancellationToken cancellationToken) {
        try { await connection.OpenAsync(cancellationToken).SkipContextSync(); }
        catch (Exception ex) {
            _logger.ConnectionCheckFailure(ex);

            return Error.Failure("Database health check connection failed.");
        }

        return true;
    }

    private async Task<Result<bool>> ExecuteSqlAsync(DbConnection connection, CancellationToken cancellationToken) {
        if (string.IsNullOrWhiteSpace(_options.Value.Sql)) {
            _logger.MissingSqlCheck();

            return true;
        }

        try {
            await using var command = connection.CreateCommand();

            command.CommandText = _options.Value.Sql;

            _ = await command.ExecuteReaderAsync(cancellationToken).SkipContextSync();
        }
        catch (Exception ex) {
            _logger.ExecuteSqlFailure(ex);

            return Error.Failure("Database health check SQL execution failed.");
        }

        return true;
    }
}