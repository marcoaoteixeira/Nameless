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
    ///     Thrown when <paramref name="options"/> is <c>null</c>.
    /// </exception>
    public DbConnectionFactory(IOptions<SqliteOptions> options) {
        _options = Prevent.Argument.Null(options);
    }

    /// <inheritdoc />
    public string ProviderName => "Sqlite";

    /// <inheritdoc />
    public IDbConnection CreateDbConnection() {
        var connectionString = _options.Value.GetConnectionString();
        var result = new SQLiteConnection(connectionString);

        return result;
    }
}