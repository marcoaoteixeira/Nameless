using System.Data;
using System.Data.SQLite;
using Microsoft.Extensions.Options;

namespace Nameless.Data.Sqlite;

/// <summary>
/// Default implementation of <see cref="IDbConnectionFactory"/> for Sqlite database.
/// </summary>
public sealed class DbConnectionFactory : IDbConnectionFactory {
    private readonly IOptions<SqliteOptions> _options;

    /// <summary>
    /// Initializes a new instance of <see cref="DbConnectionFactory"/>.
    /// </summary>
    /// <param name="options">The Sqlite options.</param>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="options"/> is <see langword="null"/>.
    /// </exception>
    public DbConnectionFactory(IOptions<SqliteOptions> options) {
        _options = Guard.Against.Null(options);
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
        var connStr = string.Empty;

        connStr += $"Data Source={(options.UseInMemory ? ":memory:" : options.DatabaseFilePath)};";
        connStr += options is { UseCredentials: true, UseInMemory: false }
            ? $"Password={options.Password};"
            : string.Empty;

        return connStr;
    }
}