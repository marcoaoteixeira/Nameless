using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Nameless.Microservices.Infrastructure.EntityFrameworkCore;
using Npgsql;

namespace Nameless.Microservices.Infrastructure.HealthChecks.Database;

public class DbConnectionFactory : IDbConnectionFactory {
    private readonly IConfiguration _configuration;
    private readonly EntityFrameworkCoreOptions _options;

    public DbConnectionFactory(
        IConfiguration configuration,
        IOptions<EntityFrameworkCoreOptions> options) {
        _configuration = configuration;
        _options = options.Value;
    }

    public DbConnection CreateConnection() {
        var connStrName = _options.ConnectionStringName;
        var connStr = _configuration.GetConnectionString(connStrName);

        if (string.IsNullOrWhiteSpace(connStr)) {
            throw new InvalidOperationException($"Missing named connection string '{connStrName}'");
        }

        return _options.Connector switch {
            EntityFrameworkCoreConnector.Sqlite => new SqliteConnection(connStr),
            EntityFrameworkCoreConnector.PostgreSQL => new NpgsqlConnection(connStr),
            _ => throw new InvalidOperationException($"Missing implementation for connector '{_options.Connector}'")
        };
    }
}