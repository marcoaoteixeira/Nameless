﻿namespace Nameless;

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
}