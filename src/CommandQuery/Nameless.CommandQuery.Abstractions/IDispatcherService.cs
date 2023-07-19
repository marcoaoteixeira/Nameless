namespace Nameless.CommandQuery {
    public interface IDispatcherService {
        #region Methods

        Task ExecuteAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
            where TCommand : ICommand;

        Task<TResult> ExecuteAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default);

        #endregion
    }
}
