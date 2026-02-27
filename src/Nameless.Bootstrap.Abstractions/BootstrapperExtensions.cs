using Nameless.Bootstrap.Infrastructure;
using Nameless.Bootstrap.Notification;
using Nameless.Null;

namespace Nameless.Bootstrap;

/// <summary>
///     <see cref="IBootstrapper"/> extension methods.
/// </summary>
public static class BootstrapperExtensions {
    /// <param name="self">
    ///     The current <see cref="IBootstrapper"/> instance.
    /// </param>
    extension(IBootstrapper self) {
        /// <summary>
        ///     Asynchronously executes the bootstrap operation.
        /// </summary>
        /// <param name="cancellationToken">
        ///     A cancellation token that can be used to cancel the operation.
        /// </param>
        /// <returns>
        ///     A task that represents the asynchronous execution operation.
        /// </returns>
        public Task ExecuteAsync(CancellationToken cancellationToken) {
            return self.ExecuteAsync(
                context: [],
                progress: NullProgress<StepProgress>.Instance,
                cancellationToken
            );
        }

        /// <summary>
        ///     Asynchronously executes the bootstrap operation.
        /// </summary>
        /// <param name="context">
        ///     The context object that provides data and state information for
        ///     the steps execution.
        /// </param>
        /// <param name="cancellationToken">
        ///     A cancellation token that can be used to cancel the operation.
        /// </param>
        /// <returns>
        ///     A task that represents the asynchronous execution operation.
        /// </returns>
        public Task ExecuteAsync(FlowContext context, CancellationToken cancellationToken) {
            return self.ExecuteAsync(
                context,
                progress: NullProgress<StepProgress>.Instance,
                cancellationToken
            );
        }

        /// <summary>
        ///     Asynchronously executes the bootstrap operation.
        /// </summary>
        /// <param name="progress">
        ///     A <see cref="IProgress{T}"/> to provide information regarding step
        ///     execution.
        /// </param>
        /// <param name="cancellationToken">
        ///     A cancellation token that can be used to cancel the operation.
        /// </param>
        /// <returns>
        ///     A task that represents the asynchronous execution operation.
        /// </returns>
        public Task ExecuteAsync(IProgress<StepProgress> progress, CancellationToken cancellationToken) {
            return self.ExecuteAsync(
                context: [],
                progress,
                cancellationToken
            );
        }
    }
}