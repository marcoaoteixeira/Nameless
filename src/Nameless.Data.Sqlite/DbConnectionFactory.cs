using System.Data;
using System.Data.SQLite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Nameless.Data.Sqlite;

/// <summary>
/// Default implementation of <see cref="IDbConnectionFactory"/> for Sqlite database.
/// </summary>
public class DbConnectionFactory : IDbConnectionFactory {
    private readonly IConfiguration _configuration;
    private readonly IOptions<SqliteOptions> _options;

    /// <summary>
    /// Initializes a new instance of <see cref="DbConnectionFactory"/>.
    /// </summary>
    /// <param name="configuration">
    ///     The configuration.
    /// </param>
    /// <param name="options">The Sqlite options.</param>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="options"/> is <see langword="null"/>.
    /// </exception>
    public DbConnectionFactory(IConfiguration configuration, IOptions<SqliteOptions> options) {
        _configuration = configuration;
        _options = options;
    }

    /// <inheritdoc />
    public string ProviderName => "Sqlite";

    /// <inheritdoc />
    public IDbConnection CreateDbConnection() {
        var connectionString = GetConnectionString();
        var result = new SQLiteConnection(connectionString);

        return result;
    }

    private string GetConnectionString() {
        var options = _options.Value;

        if (!string.IsNullOrWhiteSpace(options.ConnectionStringName)) {
            return _configuration.GetConnectionString(options.ConnectionStringName)
                   ?? throw new InvalidOperationException($"Missing named connection string: '{options.ConnectionStringName}'");
        }

        var connStr = string.Empty;

        connStr += $"Data Source={(options.UseInMemory ? ":memory:" : options.DatabaseFilePath)};";
        connStr += options is { UseCredentials: true, UseInMemory: false }
            ? $"Password={options.Password};"
            : string.Empty;

        return connStr;
    }
}