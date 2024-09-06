using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Nameless;

public static class PreventExtension {
    #region Private Constants

    private const string ARG_NULL_EX_MESSAGE = "Argument cannot be null.";
    private const string ARG_EMPTY_EX_MESSAGE = "Argument cannot be empty.";
    private const string ARG_EMPTY_WHITESPACES_EX_MESSAGE = "Argument cannot be white spaces.";
    private const string ARG_NO_MATCH_PATTER_EX_MESSAGE = "Argument does not match pattern.";
    private const string ARG_LOWER_THAN_ZERO_EX_MESSAGE = "Parameter must be positive non-zero value.";
    private const string ARG_OUT_OF_RANGE_EX_MESSAGE = "Parameter must be between min and max values.";

    #endregion

    #region Public Static Methods

    [DebuggerStepThrough]
    public static T Null<T>(this Prevent _, [NotNull] T? input, string name, string? message = null)
        => input ?? throw new ArgumentNullException(name, message ?? ARG_NULL_EX_MESSAGE);

    [DebuggerStepThrough]
    public static string NullOrEmpty(this Prevent _, [NotNull] string? input, string name, string? message = null) {
        if (input is null) {
            throw new ArgumentNullException(name, message ?? ARG_NULL_EX_MESSAGE);
        }

        if (input.Length == 0) {
            throw new ArgumentException(message ?? ARG_EMPTY_EX_MESSAGE, name);
        }

        return input;
    }

    [DebuggerStepThrough]
    public static string NullOrWhiteSpace(this Prevent _, [NotNull] string? input, string name, string? message = null) {
        if (input is null) {
            throw new ArgumentNullException(name, message ?? ARG_NULL_EX_MESSAGE);
        }

        if (input.Trim()
                 .Length == 0) {
            throw new ArgumentException(message ?? ARG_EMPTY_WHITESPACES_EX_MESSAGE, name);
        }

        return input;
    }

    [DebuggerStepThrough]
    public static T NullOrEmpty<T>(this Prevent _, [NotNull] T? input, string name, string? message = null)
        where T : class, IEnumerable {
        switch (input) {
            case null:
                throw new ArgumentNullException(name, message ?? ARG_NULL_EX_MESSAGE);

            // Costs O(1)
            case ICollection { Count: 0 }:
                throw new ArgumentException(message ?? ARG_EMPTY_EX_MESSAGE, name);
        }

        // Costs O(N)
        var enumerator = input.GetEnumerator();
        var canMoveNext = enumerator.MoveNext();
        if (enumerator is IDisposable disposable) {
            disposable.Dispose();
        }

        if (!canMoveNext) {
            throw new ArgumentException(message ?? ARG_EMPTY_EX_MESSAGE, name);
        }

        return input;
    }

    [DebuggerStepThrough]
    public static string NoMatchingPattern(this Prevent _, string input, string name, string pattern, string? message = null) {
        var match = Regex.Match(input, pattern);
        if (!match.Success || match.Value != input) {
            throw new ArgumentException(message ?? ARG_NO_MATCH_PATTER_EX_MESSAGE, name);
        }

        return input;
    }

    [DebuggerStepThrough]
    public static TimeSpan LowerThanZero(this Prevent _, TimeSpan timeSpan, string name, string? message = null) {
        if (timeSpan <= TimeSpan.Zero) {
            throw new ArgumentOutOfRangeException(paramName: name,
                                                  message: message ?? ARG_LOWER_THAN_ZERO_EX_MESSAGE);
        }

        return timeSpan;
    }

    [DebuggerStepThrough]
    public static int OutOfRange(this Prevent _, int value, int min, int max, string name, string? message = null) {
        if (value < min || value > max) {
            throw new ArgumentOutOfRangeException(paramName: name,
                                                  message: message ?? ARG_OUT_OF_RANGE_EX_MESSAGE);
        }

        return value;
    }

    #endregion
}