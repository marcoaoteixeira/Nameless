using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Nameless.Data.SqlServer;

/// <summary>
/// Default implementation of <see cref="IDbConnectionFactory"/> for MS SQL Server database.
/// </summary>
public class DbConnectionFactory : IDbConnectionFactory {
    private readonly IConfiguration _configuration;
    private readonly IOptions<SqlServerOptions> _options;

    /// <inheritdoc />
    public string ProviderName => "Microsoft SQL Server";

    /// <summary>
    /// Initializes a new instance of <see cref="DbConnectionFactory"/>.
    /// </summary>
    /// <param name="configuration">
    ///     The configuration.
    /// </param>
    /// <param name="options">The MS SQL Server options.</param>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="options"/> is <see langword="null"/>.
    /// </exception>
    public DbConnectionFactory(IConfiguration configuration, IOptions<SqlServerOptions> options) {
        _configuration = configuration;
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

        if (!string.IsNullOrWhiteSpace(options.ConnectionStringName)) {
            return _configuration.GetConnectionString(options.ConnectionStringName)
                   ?? throw new InvalidOperationException($"Missing named connection string: '{options.ConnectionStringName}'");
        }

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