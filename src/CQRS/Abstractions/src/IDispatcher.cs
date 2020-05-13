using System;
using System.Threading;
using System.Threading.Tasks;

namespace Nameless.CQRS {
    public interface IDispatcher {
        #region Methods

        /// <summary>
        /// Dispatches a command.
        /// </summary>
        /// <typeparam name="TCommand">Type of the command.</typeparam>
        /// <param name="command">The command.</param>
        /// <param name="progress">The progress notification system.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A <see cref="Task" /> representing the dispatch execution.</returns>
        Task CommandAsync<TCommand> (TCommand command, IProgress<int> progress = null, CancellationToken token = default) where TCommand : ICommand;

        /// <summary>
        /// Executes a query.
        /// </summary>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <param name="query">The query.</param>
        /// <param name="progress">The progress notification system.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A <see cref="Task{TResult}" /> representing the query execution.</returns>
        Task<TResult> QueryAsync<TResult> (IQuery<TResult> query, IProgress<int> progress = null, CancellationToken token = default);

        #endregion Methods
    }
}