using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace Nameless.Data.SqlServer;

/// <summary>
/// Default implementation of <see cref="IDbConnectionFactory"/> for MS SQL Server database.
/// </summary>
public class DbConnectionFactory : IDbConnectionFactory {
    private readonly IOptions<SqlServerOptions> _options;

    /// <inheritdoc />
    public string ProviderName => "Microsoft SQL Server";

    /// <summary>
    /// Initializes a new instance of <see cref="DbConnectionFactory"/>.
    /// </summary>
    /// <param name="options">The MS SQL Server options.</param>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="options"/> is <see langword="null"/>.
    /// </exception>
    public DbConnectionFactory(IOptions<SqlServerOptions> options) {
        _options = options;
    }

    /// <inheritdoc />
    public IDbConnection CreateDbConnection() {
        var connectionString = GetConnectionString();
        var result = new SqlConnection(connectionString);

        return result;
    }

    private string GetConnectionString() {
        var options = _options.Value;
        var connStr = string.Empty;

        connStr += $"Server={options.Server};";

        connStr += options.UseAttachedDb
            ? $"AttachDbFilename={options.Database};"
            : $"Database={options.Database};";

        connStr += options.UseCredentials
            ? $"User Id={options.Username};Password={options.Password};"
            : string.Empty;

        connStr += options.UseIntegratedSecurity
            ? "Integrated Security=true;"
            : string.Empty;

        return connStr;
    }
}