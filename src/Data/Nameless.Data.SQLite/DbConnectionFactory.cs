using System.Data;
using System.Data.SQLite;
using Microsoft.Extensions.Options;
using Nameless.Data.SQLite.Options;

namespace Nameless.Data.SQLite;

public sealed class DbConnectionFactory : IDbConnectionFactory {
    private readonly IOptions<SQLiteOptions> _options;

    public string ProviderName => "SQLite";

    public DbConnectionFactory(IOptions<SQLiteOptions> options) {
        _options = Prevent.Argument.Null(options);
    }

    public IDbConnection CreateDbConnection() {
        var connectionString = _options.Value.GetConnectionString();
        var result = new SQLiteConnection(connectionString);

        return result;
    }
}