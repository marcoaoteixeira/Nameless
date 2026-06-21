using System.Data;
using Nameless.Data.Requests;
using Nameless.Data.Responses;

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
    /// <param name="request">The request.</param>
    /// <returns>
    ///     A <see cref="ExecuteNonQueryResponse" /> instance with the
    ///     request result.
    /// </returns>
    ExecuteNonQueryResponse ExecuteNonQuery(ExecuteNonQueryRequest request);

    /// <summary>
    ///     Executes a reader query against the database.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="request">The request.</param>
    /// <returns>
    ///     A <see cref="ExecuteReaderResponse{TResult}" /> instance with the
    ///     request result.
    /// </returns>
    ExecuteReaderResponse<TResult> ExecuteReader<TResult>(ExecuteReaderRequest<TResult> request);

    /// <summary>
    ///     Executes a scalar command against the database.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>
    ///     A <see cref="ExecuteScalarResponse{TResult}" /> instance with the
    ///     request result.
    /// </returns>
    ExecuteScalarResponse<TResult> ExecuteScalar<TResult>(ExecuteScalarRequest request);
}