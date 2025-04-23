using System.Data;
using System.Data.SQLite;
using Microsoft.Extensions.Options;

namespace Nameless.Data.Sqlite;

public sealed class DbConnectionFactory : IDbConnectionFactory {
    private readonly IOptions<SqliteOptions> _options;

    public string ProviderName => "Sqlite";

    public DbConnectionFactory(IOptions<SqliteOptions> options) {
        _options = Prevent.Argument.Null(options);
    }

    public IDbConnection CreateDbConnection() {
        var connectionString = _options.Value.GetConnectionString();
        var result = new SQLiteConnection(connectionString);

        return result;
    }
}