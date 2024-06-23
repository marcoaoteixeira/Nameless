using System.Data;

namespace Nameless.Data {
    /// <summary>
    /// Implements methods to work with ADO.Net.
    /// </summary>
    public interface IDatabaseService {
        #region Methods

        /// <summary>
        /// Creates a new transaction.
        /// </summary>
        /// <param name="isolationLevel">The isolation level of the transaction.</param>
        IDbTransaction BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.Unspecified);

        /// <summary>
        /// Executes a not-query command against the database.
        /// </summary>
        /// <param name="text">The command text.</param>
        /// <param name="type">The command type. Default is <see cref="CommandType.Text"/>.</param>
        /// <param name="parameters">The command parameters.</param>
        /// <returns>An <see cref="int"/> value representing the records affected.</returns>
        int ExecuteNonQuery(string text, CommandType type = CommandType.Text, params Parameter[] parameters);

        /// <summary>
        /// Executes a reader query against the database.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="text">The command text.</param>
        /// <param name="type">The command type. Default is <see cref="CommandType.Text"/></param>
        /// <param name="mapper">The mapper for result projection.</param>
        /// <param name="parameters">The command parameters.</param>
        /// <returns>A <see cref="IEnumerable{TResult}"/> implementation instance, representing a collection of results.</returns>
        IEnumerable<TResult> ExecuteReader<TResult>(string text, Func<IDataRecord, TResult> mapper, CommandType type = CommandType.Text, params Parameter[] parameters);

        /// <summary>
        /// Executes a scalar command against the database.
        /// </summary>
        /// <param name="text">The command text.</param>
        /// <param name="type">The command type. Default is <see cref="CommandType.Text"/></param>
        /// <param name="parameters">The command parameters.</param>
        /// <returns>A <see cref="TResult"/> representing the query result.</returns>
        TResult? ExecuteScalar<TResult>(string text, CommandType type = CommandType.Text, params Parameter[] parameters);

        #endregion
    }
}