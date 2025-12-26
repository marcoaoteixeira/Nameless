using System.Runtime.CompilerServices;

namespace Nameless;

/// <summary>
///     <see cref="Task" /> extension methods.
/// </summary>
public static class TaskExtensions {
    /// <param name="self">The <see cref="Task" /> source</param>
    extension(Task self) {
        /// <summary>
        ///     Checks if the <see cref="Task" /> can continue.
        /// </summary>
        /// <returns><see langword="true"/> if it can continue; otherwise <see langword="false"/>.</returns>
        public bool CanContinue() {
            return self.Exception is null && self is {
                IsCanceled: false,
                IsFaulted: false,
                IsCompleted: true
            };
        }

        /// <summary>
        ///     Configures the awaiter used to await this
        ///     <see cref="Task"/> to do not continue on captured context.
        /// </summary>
        /// <returns>
        ///     An object used to await this task.
        /// </returns>
        /// <remarks>
        ///     This extension method executes the
        ///     <see cref="Task.ConfigureAwait(bool)"/> with the flag
        ///     <c>continueOnCapturedContext</c> set to <c>false</c>. Meaning that
        ///     the attempt to marshal the continuation back to the original context
        ///     captured should not occur.
        /// </remarks>
        public ConfiguredTaskAwaitable SkipContextSync() {
            return self.ConfigureAwait(continueOnCapturedContext: false);
        }
    }

    /// <param name="self">
    ///     The current <see cref="Task{TResult}"/>.
    /// </param>
    /// <typeparam name="TResult">
    ///     Type of the result
    /// </typeparam>
    extension<TResult>(Task<TResult> self) {
        /// <summary>
        ///     Configures the awaiter used to await this
        ///     <see cref="Task{TResult}"/> to do not continue on captured context.
        /// </summary>
        /// <returns>
        ///     An object used to await this task.
        /// </returns>
        /// <remarks>
        ///     This extension method executes the
        ///     <see cref="Task{TResult}.ConfigureAwait(bool)"/> with the flag
        ///     <c>continueOnCapturedContext</c> set to <c>false</c>. Meaning that
        ///     the attempt to marshal the continuation back to the original context
        ///     captured should not occur.
        /// </remarks>
        public ConfiguredTaskAwaitable<TResult> SkipContextSync() {
            return self.ConfigureAwait(continueOnCapturedContext: false);
        }
    }

    /// <param name="self">
    ///     The current <see cref="ValueTask{TResult}"/>.
    /// </param>
    /// <typeparam name="TResult">
    ///     Type of the result
    /// </typeparam>
    extension<TResult>(ValueTask<TResult> self) {
        /// <summary>
        ///     Configures the awaiter used to await this
        ///     <see cref="ValueTask{TResult}"/> to do not continue on captured
        ///     context.
        /// </summary>
        /// <returns>
        ///     An object used to await this task.
        /// </returns>
        /// <remarks>
        ///     This extension method executes the
        ///     <see cref="ValueTask{TResult}.ConfigureAwait(bool)"/> with the flag
        ///     <c>continueOnCapturedContext</c> set to <c>false</c>. Meaning that
        ///     the attempt to marshal the continuation back to the original context
        ///     captured should not occur.
        /// </remarks>
        public ConfiguredValueTaskAwaitable<TResult> SkipContextSync() {
            return self.ConfigureAwait(continueOnCapturedContext: false);
        }
    }

    /// <param name="self">
    ///     The current <see cref="ValueTask"/>.
    /// </param>
    extension(ValueTask self) {
        /// <summary>
        ///     Configures the awaiter used to await this
        ///     <see cref="ValueTask"/> to do not continue on captured context.
        /// </summary>
        /// <returns>
        ///     An object used to await this task.
        /// </returns>
        /// <remarks>
        ///     This extension method executes the
        ///     <see cref="ValueTask.ConfigureAwait(bool)"/> with the flag
        ///     <c>continueOnCapturedContext</c> set to <c>false</c>. Meaning that
        ///     the attempt to marshal the continuation back to the original context
        ///     captured should not occur.
        /// </remarks>
        public ConfiguredValueTaskAwaitable SkipContextSync() {
            return self.ConfigureAwait(continueOnCapturedContext: false);
        }
    }
}