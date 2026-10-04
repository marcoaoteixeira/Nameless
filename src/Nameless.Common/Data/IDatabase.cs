using System.Data;

namespace Nameless.Data;

/// <summary>
///     Implements methods to work with ADO.Net.
/// </summary>
public interface IDatabase {
    /// <summary>
    ///     Creates a new transaction.
    /// </summary>
    /// <param name="isolationLevel">The isolation level of the transaction.</param>
    IDbTransaction BeginTransaction(IsolationLevel isolationLevel);

    /// <summary>
    ///     Executes a not-query command against the database.
    /// </summary>
    /// <param name="sql">
    ///     The SQL command.
    /// </param>
    /// <param name="type">
    ///     The SQL command type.
    /// </param>
    /// <param name="parameters">
    ///     The SQL command parameters.
    /// </param>
    /// <returns>
    ///     A <see cref="ExecuteNonQueryResult" /> instance with the
    ///     request result.
    /// </returns>
    ExecuteNonQueryResult ExecuteNonQuery(string sql, CommandType type = CommandType.Text, params IEnumerable<Parameter> parameters);

    /// <summary>
    ///     Executes a reader query against the database.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of the result.
    /// </typeparam>
    /// <param name="sql">
    ///     The SQL command.
    /// </param>
    /// <param name="mapper">
    ///     The SQL result mapper function.
    /// </param>
    /// <param name="type">
    ///     The SQL command type.
    /// </param>
    /// <param name="parameters">
    ///     The SQL command parameters.
    /// </param>
    /// <returns>
    ///     A <see cref="ExecuteReaderResult{T}" /> instance with the
    ///     request result.
    /// </returns>
    ExecuteReaderResult<T> ExecuteReader<T>(string sql, Func<IDataRecord, T> mapper, CommandType type = CommandType.Text, params IEnumerable<Parameter> parameters);

    /// <summary>
    ///     Executes a scalar command against the database.
    /// </summary>
    /// <param name="sql">
    ///     The SQL command.
    /// </param>
    /// <param name="type">
    ///     The SQL command type.
    /// </param>
    /// <param name="parameters">
    ///     The SQL command parameters.
    /// </param>
    /// <returns>
    ///     A <see cref="ExecuteScalarResult{T}" /> instance with the
    ///     request result.
    /// </returns>
    ExecuteScalarResult<T> ExecuteScalar<T>(string sql, CommandType type = CommandType.Text, params IEnumerable<Parameter> parameters);
}