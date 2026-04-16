using System.Data;

namespace Nameless.Data;

/// <summary>
///     Provides a factory for creating database connections.
/// </summary>
public interface IDbConnectionFactory {
    /// <summary>
    /// Gets the name of the database provider.
    /// </summary>
    string ProviderName { get; }

    /// <summary>
    ///     Creates a database connection.
    /// </summary>
    /// <returns>
    ///     An instance of <see cref="IDbConnection" /> implemented by
    ///     the provider defined by <see cref="ProviderName" />.
    /// </returns>
    IDbConnection CreateDbConnection();
}