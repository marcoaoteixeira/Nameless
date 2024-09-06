using System.Data;
using Microsoft.Data.SqlClient;
using Nameless.Data.SQLServer.Options;

namespace Nameless.Data.SQLServer;

public sealed class DbConnectionFactory : IDbConnectionFactory {
    private readonly SQLServerOptions _options;
    
    public string ProviderName => "Microsoft SQL Server";

    public DbConnectionFactory(SQLServerOptions options) {
        _options = Prevent.Argument.Null(options, nameof(options));
    }

    public IDbConnection CreateDbConnection() {
        var connectionString = _options.GetConnectionString();
        var result = new SqlConnection(connectionString);

        return result;
    }
}