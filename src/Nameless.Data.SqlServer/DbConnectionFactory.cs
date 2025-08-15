using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace Nameless.Data.SqlServer;

/// <summary>
/// Default implementation of <see cref="IDbConnectionFactory"/> for MS SQL Server database.
/// </summary>
public sealed class DbConnectionFactory : IDbConnectionFactory {
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
        _options = Guard.Against.Null(options);
    }

    /// <inheritdoc />
    public IDbConnection CreateDbConnection() {
        var connectionString = _options.Value.GetConnectionString();
        var result = new SqlConnection(connectionString);

        return result;
    }
}