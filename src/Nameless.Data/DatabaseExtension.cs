using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Dasync.Collections;

namespace Nameless.Data {

    /// <summary>
    /// Extension methods for <see cref="IDatabase"/>
    /// </summary>
    public static class DatabaseExtension {

        #region Public Static Methods

        /// <summary>
        /// Executes a reader query against the data base, and returns only one result.
        /// If more than one result was found, throws exception.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="source">The <see cref="IDatabase"/> instance.</param>
        /// <param name="commandText">The command text.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="mapper">The mapper for result projection.</param>
        /// <param name="token">The cancellation token.</param>
        /// <param name="parameters">The command parameters.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the method execution.</returns>
        public static Task<TResult> ExecuteReaderSingleAsync<TResult> (this IDatabase source, string commandText, Func<IDataRecord, TResult> mapper, CommandType commandType = CommandType.Text, CancellationToken token = default, params Parameter[] parameters) {
            if (source == null) { return default; }

            return source.ExecuteReaderAsync (commandText, mapper, commandType, parameters).FirstOrDefaultAsync (token);
        }

        /// <summary>
        /// Executes a scalar command against the data base.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="source">The <see cref="IDatabase"/> instance.</param>
        /// <param name="commandText">The command text.</param>
        /// <param name="commandType">The command type.</param>
        /// 
        /// <param name="parameters">The command parameters.</param>
        /// <returns>A instance of <typeparamref name="TResult"/>.</returns>
        public static Task<TResult> ExecuteScalarAsync<TResult> (this IDatabase source, string commandText, CommandType commandType = CommandType.Text, CancellationToken token = default, params Parameter[] parameters) {
            if (source == null) { return default; }

            return source
                .ExecuteScalarAsync (commandText, commandType, token, parameters)
                .ContinueWith (continuation => {
                    TResult result = default;
                    if (continuation.CanContinue () && continuation.Result != null) {
                        result = (TResult)continuation.Result;
                    }
                    return result;
                });
        }

        #endregion
    }
}