namespace Nameless;

/// <summary>
/// <see cref="Task"/> extension methods.
/// </summary>
public static class TaskExtension {
    /// <summary>
    /// Checks if the <see cref="Task"/> can continue.
    /// </summary>
    /// <param name="self">The <see cref="Task"/> source</param>
    /// <returns><c>true</c> if it can continue; otherwise <c>false</c>.</returns>
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="self"/> is <c>null</c>.
    /// </exception>
    public static bool CanContinue(this Task self) {
        Prevent.Argument.Null(self);

        return self.Exception is null && self is { IsCanceled: false, IsFaulted: false, IsCompleted: true };
    }
}