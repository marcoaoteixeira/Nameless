namespace Nameless.CommandQuery {

    public interface IQueryDispatcher {

        #region Methods

        /// <summary>
        /// Executes a query.
        /// </summary>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <param name="query">The query.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task{TResult}" /> representing the query execution.</returns>
        Task<TResult> ExecuteAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default);

        #endregion
    }
}
