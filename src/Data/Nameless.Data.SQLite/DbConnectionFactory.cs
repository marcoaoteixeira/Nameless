using System.Data;
using System.Data.SQLite;
using Nameless.Data.SQLite.Options;

namespace Nameless.Data.SQLite;

public sealed class DbConnectionFactory : IDbConnectionFactory {
    private readonly SQLiteOptions _options;

    public string ProviderName => "SQLite";

    public DbConnectionFactory(SQLiteOptions options) {
        _options = Prevent.Argument.Null(options, nameof(options));
    }

    public IDbConnection CreateDbConnection() {
        var connectionString = _options.GetConnectionString();
        var result = new SQLiteConnection(connectionString);

        return result;
    }
}