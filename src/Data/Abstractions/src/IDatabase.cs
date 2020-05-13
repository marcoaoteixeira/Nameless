using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Nameless.Data {

    /// <summary>
    /// Defines methods/properties/events to implement a database accessor to work with ADO.Net
    /// </summary>
    public interface IDatabase : IDisposable {

        #region Properties

        /// <summary>
        /// Gets the database provider name.
        /// </summary>
        string ProviderName { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Starts a database transaction and returns a reference to the transaction to caller.
        /// If a transaction was already started, returns its reference.
        /// </summary>
        /// <param name="level">The isolation level.</param>
        /// <returns>A reference to the transaction object.</returns>
        IDbTransaction StartTransaction (IsolationLevel level = IsolationLevel.Unspecified);

        /// <summary>
        /// Executes a not-query command against the data base.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <param name="commandType">The command type. Default is <see cref="CommandType.Text"/>.</param>
        /// <param name="token">The cancellation token.</param>
        /// <param name="parameters">The command parameters.</param>
        /// <returns>A <see cref="Task{int}"/> representing the method execution.</returns>
        Task<int> ExecuteNonQueryAsync (string commandText, CommandType commandType = CommandType.Text, CancellationToken token = default, params Parameter[] parameters);

        /// <summary>
        /// Executes a reader query against the data base.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="commandText">The command text.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="mapper">The mapper for result projection.</param>
        /// <param name="parameters">The command parameters.</param>
        /// <returns>A <see cref="IAsyncEnumerable{TResult}"/> representing a collection of results.</returns>
        IAsyncEnumerable<TResult> ExecuteReaderAsync<TResult> (string commandText, Func<IDataRecord, TResult> mapper, CommandType commandType = CommandType.Text, params Parameter[] parameters);

        /// <summary>
        /// Executes a scalar command against the data base.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="token">The cancellation token.</param>
        /// <param name="parameters">The command parameters.</param>
        /// <returns>A <see cref="Task{object}"/> representing the method execution.</returns>
        Task<object> ExecuteScalarAsync (string commandText, CommandType commandType = CommandType.Text, CancellationToken token = default, params Parameter[] parameters);

        #endregion
    }
}