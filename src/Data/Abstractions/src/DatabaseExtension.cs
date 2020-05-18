using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

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
        /// <param name="self">The <see cref="IDatabase"/> instance.</param>
        /// <param name="commandText">The command text.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="mapper">The mapper for result projection.</param>
        /// <param name="token">The cancellation token.</param>
        /// <param name="parameters">The command parameters.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the method execution.</returns>
        public static async Task<TResult> ExecuteReaderSingleAsync<TResult> (this IDatabase self, string commandText, Func<IDataRecord, TResult> mapper, CommandType commandType = CommandType.Text, CancellationToken token = default, params Parameter[] parameters) {
            if (self == null) { return default; }

            var enumerator = self.ExecuteReaderAsync (commandText, mapper, commandType, parameters).GetAsyncEnumerator (token);
            var result = await enumerator.MoveNextAsync () ? enumerator.Current : default;

            await enumerator.DisposeAsync ();

            return result;
        }

        #endregion
    }
}