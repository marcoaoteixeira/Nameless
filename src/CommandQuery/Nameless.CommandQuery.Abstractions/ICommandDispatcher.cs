using Nameless.Infrastructure;

namespace Nameless.CommandQuery {

    public interface ICommandDispatcher {

        #region Methods

        /// <summary>
        /// Executes a command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task{ExecutionResult}" /> representing the command execution.</returns>
        Task<ExecutionResult> ExecuteAsync(ICommand command, CancellationToken cancellationToken = default);

        #endregion
    }
}
