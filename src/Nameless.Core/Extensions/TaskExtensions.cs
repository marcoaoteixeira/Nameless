using System.Runtime.CompilerServices;

namespace Nameless;

/// <summary>
///     <see cref="Task" /> extension methods.
/// </summary>
public static class TaskExtensions {
    /// <summary>
    ///     Checks if the <see cref="Task" /> can continue.
    /// </summary>
    /// <param name="self">The <see cref="Task" /> source</param>
    /// <returns><see langword="true"/> if it can continue; otherwise <see langword="false"/>.</returns>
    public static bool CanContinue(this Task self) {
        return self.Exception is null && self is { IsCanceled: false, IsFaulted: false, IsCompleted: true };
    }

    /// <summary>
    ///     Configures the awaiter used to await this
    ///     <see cref="Task{TResult}"/> to do not continue on captured context.
    /// </summary>
    /// <typeparam name="TResult">
    ///     Type of the result
    /// </typeparam>
    /// <param name="self">
    ///     The current <see cref="Task{TResult}"/>.
    /// </param>
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
    public static ConfiguredTaskAwaitable<TResult> SuppressContext<TResult>(this Task<TResult> self) {
        return self.ConfigureAwait(continueOnCapturedContext: false);
    }

    /// <summary>
    ///     Configures the awaiter used to await this
    ///     <see cref="Task"/> to do not continue on captured context.
    /// </summary>
    /// <param name="self">
    ///     The current <see cref="Task"/>.
    /// </param>
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
    public static ConfiguredTaskAwaitable SuppressContext(this Task self) {
        return self.ConfigureAwait(continueOnCapturedContext: false);
    }

    /// <summary>
    ///     Configures the awaiter used to await this
    ///     <see cref="ValueTask{TResult}"/> to do not continue on captured
    ///     context.
    /// </summary>
    /// <typeparam name="TResult">
    ///     Type of the result
    /// </typeparam>
    /// <param name="self">
    ///     The current <see cref="ValueTask{TResult}"/>.
    /// </param>
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
    public static ConfiguredValueTaskAwaitable<TResult> SuppressContext<TResult>(this ValueTask<TResult> self) {
        return self.ConfigureAwait(continueOnCapturedContext: false);
    }

    /// <summary>
    ///     Configures the awaiter used to await this
    ///     <see cref="ValueTask"/> to do not continue on captured context.
    /// </summary>
    /// <param name="self">
    ///     The current <see cref="ValueTask"/>.
    /// </param>
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
    public static ConfiguredValueTaskAwaitable SuppressContext(this ValueTask self) {
        return self.ConfigureAwait(continueOnCapturedContext: false);
    }
}