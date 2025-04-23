using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace Nameless.Data.SqlServer;

public sealed class DbConnectionFactory : IDbConnectionFactory {
    private readonly IOptions<SqlServerOptions> _options;
    
    public string ProviderName => "Microsoft SQL Server";

    public DbConnectionFactory(IOptions<SqlServerOptions> options) {
        _options = Prevent.Argument.Null(options);
    }

    public IDbConnection CreateDbConnection() {
        var connectionString = _options.Value.GetConnectionString();
        var result = new SqlConnection(connectionString);

        return result;
    }
}