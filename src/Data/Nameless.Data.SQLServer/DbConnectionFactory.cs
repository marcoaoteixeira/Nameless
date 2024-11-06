using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Nameless.Data.SQLServer.Options;

namespace Nameless.Data.SQLServer;

public sealed class DbConnectionFactory : IDbConnectionFactory {
    private readonly IOptions<SQLServerOptions> _options;
    
    public string ProviderName => "Microsoft SQL Server";

    public DbConnectionFactory(IOptions<SQLServerOptions> options) {
        _options = Prevent.Argument.Null(options);
    }

    public IDbConnection CreateDbConnection() {
        var connectionString = _options.Value.GetConnectionString();
        var result = new SqlConnection(connectionString);

        return result;
    }
}