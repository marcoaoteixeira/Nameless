using System.Data;

namespace Nameless.Data;

public interface IDbConnectionFactory {
    string ProviderName { get; }

    /// <summary>
    /// Creates a database connection.
    /// </summary>
    /// <returns>
    /// An instance of <see cref="IDbConnection" /> implemented by
    /// the provider defined by <see cref="ProviderName"/>.
    /// </returns>
    IDbConnection CreateDbConnection();
}