using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Nameless;

public static class PreventExtension {
    private const string ARG_NULL_EX_MESSAGE = "Argument cannot be null.";
    private const string ARG_EMPTY_EX_MESSAGE = "Argument cannot be empty.";
    private const string ARG_EMPTY_WHITESPACES_EX_MESSAGE = "Argument cannot be white spaces.";
    private const string ARG_NO_MATCH_PATTER_EX_MESSAGE = "Argument does not match pattern.";
    private const string ARG_LOWER_OR_EQUAL_EX_MESSAGE = "Parameter cannot be lower or equal to compare value.";
    private const string ARG_OUT_OF_RANGE_EX_MESSAGE = "Parameter must be between min and max values.";

    [DebuggerStepThrough]
    public static TValue Null<TValue>(this Prevent _, [NotNull] TValue? paramValue, [CallerArgumentExpression(nameof(paramValue))] string? paramName = null, string? message = null)
        where TValue : class
        => paramValue ?? throw new ArgumentNullException(paramName, message ?? ARG_NULL_EX_MESSAGE);

    [DebuggerStepThrough]
    public static string NullOrEmpty(this Prevent _, [NotNull] string? paramValue, [CallerArgumentExpression(nameof(paramValue))] string? paramName = null, string? message = null) {
        if (paramValue is null) {
            throw new ArgumentNullException(paramName, message ?? ARG_NULL_EX_MESSAGE);
        }

        if (paramValue.Length == 0) {
            throw new ArgumentException(message ?? ARG_EMPTY_EX_MESSAGE, paramName);
        }

        return paramValue;
    }

    [DebuggerStepThrough]
    public static string NullOrWhiteSpace(this Prevent _, [NotNull] string? paramValue, [CallerArgumentExpression(nameof(paramValue))] string? paramName = null, string? message = null) {
        if (paramValue is null) {
            throw new ArgumentNullException(paramName, message ?? ARG_NULL_EX_MESSAGE);
        }

        if (paramValue.Trim().Length == 0) {
            throw new ArgumentException(message ?? ARG_EMPTY_WHITESPACES_EX_MESSAGE, paramName);
        }

        return paramValue;
    }

    [DebuggerStepThrough]
    public static T NullOrEmpty<T>(this Prevent _, [NotNull] T? paramValue, [CallerArgumentExpression(nameof(paramValue))] string? paramName = null, string? message = null)
        where T : class, IEnumerable {
        switch (paramValue) {
            case null:
                throw new ArgumentNullException(paramName, message ?? ARG_NULL_EX_MESSAGE);

            // Costs O(1)
            case ICollection { Count: 0 }:
                throw new ArgumentException(message ?? ARG_EMPTY_EX_MESSAGE, paramName);
        }

        // Costs O(N)
        var enumerator = paramValue.GetEnumerator();
        var canMoveNext = enumerator.MoveNext();
        if (enumerator is IDisposable disposable) {
            disposable.Dispose();
        }

        if (!canMoveNext) {
            throw new ArgumentException(message ?? ARG_EMPTY_EX_MESSAGE, paramName);
        }

        return paramValue;
    }

    [DebuggerStepThrough]
    public static string NoMatchingPattern(this Prevent _, string paramValue, string pattern, [CallerArgumentExpression(nameof(paramValue))] string? paramName = null, string? message = null) {
        var match = Regex.Match(paramValue, pattern);
        if (!match.Success || match.Value != paramValue) {
            throw new ArgumentException(message ?? ARG_NO_MATCH_PATTER_EX_MESSAGE, paramName);
        }

        return paramValue;
    }

    [DebuggerStepThrough]
    public static TimeSpan LowerOrEqual(this Prevent _, TimeSpan paramValue, TimeSpan to, [CallerArgumentExpression(nameof(paramValue))] string? paramName = null, string? message = null) {
        if (paramValue <= to) {
            throw new ArgumentOutOfRangeException(paramName: paramName,
                                                  message: message ?? ARG_LOWER_OR_EQUAL_EX_MESSAGE,
                                                  actualValue: paramValue);
        }

        return paramValue;
    }

    [DebuggerStepThrough]
    public static int LowerOrEqual(this Prevent _, int paramValue, int to, [CallerArgumentExpression(nameof(paramValue))] string? paramName = null, string? message = null) {
        if (paramValue <= to) {
            throw new ArgumentOutOfRangeException(paramName: paramName,
                                                  message: message ?? ARG_LOWER_OR_EQUAL_EX_MESSAGE,
                                                  actualValue: paramValue);
        }

        return paramValue;
    }

    [DebuggerStepThrough]
    public static int LowerThan(this Prevent _, int paramValue, int minValue, [CallerArgumentExpression(nameof(paramValue))] string? paramName = null, string? message = null) {
        if (paramValue < minValue) {
            throw new ArgumentOutOfRangeException(paramName: paramName,
                                                  message: message ?? ARG_LOWER_OR_EQUAL_EX_MESSAGE,
                                                  actualValue: paramValue);
        }

        return paramValue;
    }

    [DebuggerStepThrough]
    public static int OutOfRange(this Prevent _, int paramValue, int min, int max, [CallerArgumentExpression(nameof(paramValue))] string? paramName = null, string? message = null) {
        if (paramValue < min || paramValue > max) {
            throw new ArgumentOutOfRangeException(paramName: paramName,
                                                  message: message ?? ARG_OUT_OF_RANGE_EX_MESSAGE,
                                                  actualValue: paramValue);
        }

        return paramValue;
    }
}