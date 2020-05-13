using System;
using System.Threading;
using System.Threading.Tasks;

namespace Nameless.Persistence {

    /// <summary>
    /// Defines methods for directives.
    /// A directive can be a procedure, for example.
    /// /// </summary>
    /// <typeparam name="TResult">Type of the result</typeparam>
	public interface IDirective<TResult> {

        #region Methods

        /// <summary>
        /// Executes the directive.
        /// </summary>
        /// <param name="parameters">The directive parameters.</param>
        /// <param name="progress">The progress notifier, if any.</param>
        /// <param name="token">The cancellation token, if any.</param>
        /// <returns>A dynamic representing the directive execution.</returns>
        Task<TResult> ExecuteAsync(NameValueParameterSet parameters, IProgress<int> progress = null, CancellationToken token = default);

        #endregion Methods
    }
}