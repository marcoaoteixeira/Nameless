﻿namespace Nameless;

/// <summary>
///     <see cref="int" /> extension methods.
/// </summary>
public static class IntegerExtensions {
    /// <summary>
    ///     Runs a loop by the number of times that the <paramref name="self" /> parameter informs.
    ///     Using the action specified.
    /// </summary>
    /// <param name="self">The self <see cref="int" />.</param>
    /// <param name="action">The action of the interaction.</param>
    public static void Times(this int self, Action action) {
        self.Times(_ => action());
    }

    /// <summary>
    ///     Runs a loop by the number of times that the <paramref name="self" /> parameter informs.
    ///     Using the action specified. In this case, the action receives a parameter, that will be
    ///     the index of the interaction.
    /// </summary>
    /// <param name="self">The self <see cref="int" />.</param>
    /// <param name="action">The action of the interaction.</param>
    public static void Times(this int self, Action<int> action) {
        for (var number = 0; number < self; number++) {
            action(number);
        }
    }

    /// <summary>
    ///     Checks if the current <see cref="int" /> parameter falls between
    ///     the minimum and maximum defined values.
    /// </summary>
    /// <param name="self">The current <see cref="int" /> value.</param>
    /// <param name="min">The minimum value.</param>
    /// <param name="max">The maximum value.</param>
    /// <param name="includeLimit">
    ///     Whether it should check if the value is <paramref name="min" /> and
    ///     <paramref name="max" /> limits.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if it falls between; otherwise <see langword="false"/>.
    /// </returns>
    public static bool IsWithinRange(this int self, int min, int max, bool includeLimit = true) {
        return includeLimit
            ? self >= min && self <= max
            : self > min && self < max;
    }
}