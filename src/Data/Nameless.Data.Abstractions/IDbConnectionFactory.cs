using System.Data;

namespace Nameless.Data {
    public interface IDbConnectionFactory {
        #region Properties

        string ProviderName { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Creates a database connection.
        /// </summary>
        /// <returns>An instance of <see cref="IDbConnection" />.</returns>
        IDbConnection CreateDbConnection();

        #endregion
    }
}