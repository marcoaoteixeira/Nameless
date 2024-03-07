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
        /// <returns>
        /// An instance of <see cref="IDbConnection" /> implemented by
        /// the provider defined by <see cref="ProviderName"/>.
        /// </returns>
        IDbConnection CreateDbConnection();

        #endregion
    }
}