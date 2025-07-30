#pragma warning disable CA1822

using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Nameless;

/// <summary>
///     A guard clause (https://en.wikipedia.org/wiki/Guard_(computer_science))
///     is a software pattern that simplifies complex functions by
///     "failing fast", checking for invalid inputs up front and immediately
///     failing if any are found.
/// </summary>
public sealed class Prevent {
    private const string ARG_NULL_MESSAGE = "Argument cannot be null";
    private const string ARG_EMPTY_MESSAGE = "Argument cannot be empty";
    private const string ARG_DEFAULT_MESSAGE = "Argument cannot be default";
    private const string ARG_WHITE_SPACES_MESSAGE = "Argument cannot be white spaces";
    private const string ARG_NO_MATCHING_PATTERN_MESSAGE = "Argument does not match pattern: {0}";
    private const string ARG_LOWER_OR_EQUAL_MESSAGE = "Argument cannot be lower or equal to {0}";
    private const string ARG_GREATER_OR_EQUAL_MESSAGE = "Argument cannot be greater or equal to {0}";
    private const string ARG_LOWER_THAN_MESSAGE = "Argument cannot be lower than {0}";
    private const string ARG_GREATER_THAN_MESSAGE = "Argument cannot be greater than {0}";
    private const string ARG_OUT_OF_RANGE_MESSAGE = "Argument must be between min ({0}) and max ({1}) values";
    private const string ARG_ZERO_MESSAGE = "Argument cannot be zero";
    private const string ARG_UNDEFINED_ENUM_MESSAGE = "Argument '{0}' provides value '{1}' that is not defined in enum '{2}'";
    private const string ARG_NOT_ASSIGNABLE_FROM_MESSAGE = "Type of the argument '{0}' ('{1}') is not assignable from '{2}'.";

    /// <summary>
    ///     Gets the unique instance of <see cref="Prevent" />.
    /// </summary>
    public static Prevent Argument { get; } = new();

    // Explicit static constructor to tell the C# compiler
    // not to mark type as beforefieldinit
    static Prevent() { }

    private Prevent() { }

    /// <summary>
    ///     Ensure that the <paramref name="paramValue"/> is
    ///     not <see langword="null"/>. Where the <paramref name="paramValue"/>
    ///     is a reference type.
    /// </summary>
    /// <typeparam name="TValue">
    ///     Type of value.
    /// </typeparam>
    /// <param name="paramValue">
    ///     The parameter value.
    /// </param>
    /// <param name="paramName">
    ///     The parameter name (optional).
    /// </param>
    /// <param name="message">
    ///     The exception message (optional).
    /// </param>
    /// <param name="exceptionCreator">
    ///     The exception creator (optional).
    /// </param>
    /// <returns>
    ///     The current <paramref name="paramValue"/>.
    /// </returns>
    [DebuggerStepThrough]
    public TValue Null<TValue>([NotNull] TValue? paramValue,
        [CallerArgumentExpression(nameof(paramValue))]
        string? paramName = null,
        string? message = null,
        Func<Exception>? exceptionCreator = null) where TValue : class {
        if (paramValue is not null) {
            return paramValue;
        }

        throw exceptionCreator?.Invoke()
              ?? new ArgumentNullException(paramName, string.IsNullOrWhiteSpace(message)
                  ? ARG_NULL_MESSAGE
                  : message);
    }

    /// <summary>
    ///     Ensure that the <paramref name="paramValue"/> is
    ///     not <see langword="null"/>. Where the <paramref name="paramValue"/>
    ///     is a value type.
    /// </summary>
    /// <typeparam name="TValue">
    ///     Type of value.
    /// </typeparam>
    /// <param name="paramValue">
    ///     The parameter value.
    /// </param>
    /// <param name="paramName">
    ///     The parameter name (optional).
    /// </param>
    /// <param name="message">
    ///     The exception message (optional).
    /// </param>
    /// <param name="exceptionCreator">
    ///     The exception creator (optional).
    /// </param>
    /// <returns>
    ///     The current <paramref name="paramValue"/>.
    /// </returns>
    [DebuggerStepThrough]
    public TValue Null<TValue>([NotNull] TValue? paramValue,
        [CallerArgumentExpression(nameof(paramValue))]
        string? paramName = null,
        string? message = null,
        Func<Exception>? exceptionCreator = null) where TValue : struct {
        if (paramValue is not null) {
            return paramValue.Value;
        }

        throw exceptionCreator?.Invoke()
              ?? new ArgumentNullException(paramName, string.IsNullOrWhiteSpace(message)
                  ? ARG_NULL_MESSAGE
                  : message);
    }

    /// <summary>
    ///     Ensure that the <paramref name="paramValue"/> is not
    ///     <see langword="null"/> or empty space.
    /// </summary>
    /// <param name="paramValue">
    ///     The parameter value.
    /// </param>
    /// <param name="paramName">
    ///     The parameter name (optional).
    /// </param>
    /// <param name="message">
    ///     The exception message (optional).
    /// </param>
    /// <param name="exceptionCreator">
    ///     The exception creator (optional).
    /// </param>
    /// <returns>
    ///     The current <paramref name="paramValue"/>.
    /// </returns>
    [DebuggerStepThrough]
    public string NullOrEmpty([NotNull] string? paramValue,
        [CallerArgumentExpression(nameof(paramValue))]
        string? paramName = null,
        string? message = null,
        Func<Exception>? exceptionCreator = null) {
        Null(paramValue, paramName, message, exceptionCreator);

        if (paramValue.Length > 0) {
            return paramValue;
        }

        throw exceptionCreator?.Invoke()
              ?? new ArgumentException(string.IsNullOrWhiteSpace(message)
                      ? ARG_EMPTY_MESSAGE
                      : message,
                  paramName);
    }

    /// <summary>
    ///     Ensure that the <paramref name="paramValue"/> is not
    ///     <see langword="null"/>, empty or only whitespace.
    /// </summary>
    /// <param name="paramValue">
    ///     The parameter value.
    /// </param>
    /// <param name="paramName">
    ///     The parameter name (optional).
    /// </param>
    /// <param name="message">
    ///     The message (optional).
    /// </param>
    /// <param name="exceptionCreator">
    ///     The exception creator (optional).
    /// </param>
    /// <returns>
    ///     The current <paramref name="paramValue"/>.
    /// </returns>
    [DebuggerStepThrough]
    public string NullOrWhiteSpace([NotNull] string? paramValue,
        [CallerArgumentExpression(nameof(paramValue))]
        string? paramName = null,
        string? message = null,
        Func<Exception>? exceptionCreator = null) {
        NullOrEmpty(paramValue, paramName, message, exceptionCreator);

        if (paramValue.Trim().Length > 0) {
            return paramValue;
        }

        throw exceptionCreator?.Invoke()
              ?? new ArgumentException(string.IsNullOrWhiteSpace(message)
                      ? ARG_WHITE_SPACES_MESSAGE
                      : message,
                  paramName);
    }

    /// <summary>
    ///     Ensure that the <paramref name="paramValue"/> is not
    ///     <see langword="null"/> or empty
    /// </summary>
    /// <param name="paramValue">
    ///     The parameter value.
    /// </param>
    /// <param name="paramName">
    ///     The parameter name (optional).
    /// </param>
    /// <param name="message">
    ///     The exception message (optional).
    /// </param>
    /// <param name="exceptionCreator">
    ///     The exception creator (optional).
    /// </param>
    /// <returns>
    ///     The current <paramref name="paramValue"/>.
    /// </returns>
    [DebuggerStepThrough]
    public Guid NullOrEmpty([NotNull] Guid? paramValue,
        [CallerArgumentExpression(nameof(paramValue))]
        string? paramName = null,
        string? message = null,
        Func<Exception>? exceptionCreator = null) {
        Null(paramValue, paramName, message, exceptionCreator);

        if (paramValue == Guid.Empty) {
            throw exceptionCreator?.Invoke() ?? new ArgumentException(string.IsNullOrWhiteSpace(message)
                    ? ARG_EMPTY_MESSAGE
                    : message,
                paramName);
        }

        return paramValue.Value;
    }

    /// <summary>
    ///     Ensure that the <paramref name="paramValue"/> is not
    ///     <see langword="null"/> or empty.
    ///     The <paramref name="paramValue"/> must implements
    ///     <see cref="IEnumerable"/>.
    /// </summary>
    /// <param name="paramValue">
    ///     The parameter value.
    /// </param>
    /// <param name="paramName">
    ///     The parameter name (optional).
    /// </param>
    /// <param name="message">
    ///     The exception message (optional).
    /// </param>
    /// <param name="exceptionCreator">
    ///     The exception creator (optional).
    /// </param>
    /// <returns>
    ///     The current <paramref name="paramValue"/>.
    /// </returns>
    [SuppressMessage(category: "ReSharper", checkId: "PossibleMultipleEnumeration")]
    [DebuggerStepThrough]
    public TValue NullOrEmpty<TValue>([NotNull] TValue? paramValue,
        [CallerArgumentExpression(nameof(paramValue))]
        string? paramName = null,
        string? message = null,
        Func<Exception>? exceptionCreator = null) where TValue : class, IEnumerable {
        Null(paramValue, paramName, message, exceptionCreator);

        if (paramValue is Array { Length: 0 } or ICollection { Count: 0 }) {
            ThrowError(paramName, message, exceptionCreator);
        }

        // Unfortunately, it needs to enumerate here =/
        var enumerator = paramValue.GetEnumerator();
        var canMoveNext = enumerator.MoveNext();
        if (enumerator is IDisposable disposable) {
            disposable.Dispose();
        }

        if (!canMoveNext) {
            ThrowError(paramName, message, exceptionCreator);
        }

        return paramValue;

        static void ThrowError(string? paramName, string? message, Func<Exception>? exceptionCreator) {
            throw exceptionCreator?.Invoke()
                  ?? new ArgumentException(string.IsNullOrWhiteSpace(message)
                          ? ARG_EMPTY_MESSAGE
                          : message,
                      paramName);
        }
    }

    /// <summary>
    ///     Ensure that the <paramref name="paramValue"/> is not
    ///     default.
    /// </summary>
    /// <param name="paramValue">
    ///     The parameter value.
    /// </param>
    /// <param name="paramName">
    ///     The parameter name (optional).
    /// </param>
    /// <param name="message">
    ///     The exception message (optional).
    /// </param>
    /// <param name="exceptionCreator">
    ///     The exception creator (optional).
    /// </param>
    /// <returns>
    ///     The current <paramref name="paramValue"/>.
    /// </returns>
    [DebuggerStepThrough]
    public TValue Default<TValue>([NotNull] TValue? paramValue,
        [CallerArgumentExpression(nameof(paramValue))]
        string? paramName = null,
        string? message = null,
        Func<Exception>? exceptionCreator = null) {

        if (!EqualityComparer<TValue?>.Default.Equals(paramValue, y: default) && paramValue is not null) {
            return paramValue;
        }

        throw exceptionCreator?.Invoke()
              ?? new ArgumentException(string.IsNullOrWhiteSpace(message)
                      ? ARG_DEFAULT_MESSAGE
                      : message,
                  paramName);
    }

    /// <summary>
    ///     Ensure that the <paramref name="paramValue"/> matches
    ///     the specified <paramref name="pattern"/>.
    /// </summary>
    /// <param name="paramValue">
    ///     The parameter value.
    /// </param>
    /// <param name="pattern">
    ///     The pattern to match.
    /// </param>
    /// <param name="paramName">
    ///     The parameter name (optional).
    /// </param>
    /// <param name="message">
    ///     The exception message (optional).
    /// </param>
    /// <param name="exceptionCreator">
    ///     The exception creator (optional).
    /// </param>
    /// <returns>
    ///     The current <paramref name="paramValue"/>.
    /// </returns>
    [DebuggerStepThrough]
    public string NoMatchingPattern(string paramValue,
        string pattern,
        [CallerArgumentExpression(nameof(paramValue))]
        string? paramName = null,
        string? message = null,
        Func<Exception>? exceptionCreator = null) {
        var match = Regex.Match(paramValue, pattern);
        if (match.Success && match.Value == paramValue) {
            return paramValue;
        }

        throw exceptionCreator?.Invoke()
              ?? new ArgumentException(string.IsNullOrWhiteSpace(message)
                      ? string.Format(ARG_NO_MATCHING_PATTERN_MESSAGE, pattern)
                      : message,
                  paramName);
    }

    /// <summary>
    ///     Ensure that the <paramref name="paramValue"/> is not empty.
    /// </summary>
    /// <param name="paramValue">
    ///     The parameter value.
    /// </param>
    /// <param name="paramName">
    ///     The parameter name (optional).
    /// </param>
    /// <param name="message">
    ///     The exception message (optional).
    /// </param>
    /// <param name="exceptionCreator">
    ///     The exception creator (optional).
    /// </param>
    /// <returns>
    ///     The current <paramref name="paramValue"/>.
    /// </returns>
    [DebuggerStepThrough]
    public ReadOnlySpan<char> Empty(ReadOnlySpan<char> paramValue,
        [CallerArgumentExpression(nameof(paramValue))]
        string? paramName = null,
        string? message = null,
        Func<Exception>? exceptionCreator = null) {
        if (paramValue.Length > 0 && paramValue != string.Empty) {
            return paramValue;
        }

        throw exceptionCreator?.Invoke()
              ?? new ArgumentException(string.IsNullOrWhiteSpace(message)
                      ? ARG_EMPTY_MESSAGE
                      : message,
                  paramName);
    }

    /// <summary>
    ///     Ensure that the <paramref name="paramValue"/> is not only
    ///     whitespace.
    /// </summary>
    /// <param name="paramValue">
    ///     The parameter value.
    /// </param>
    /// <param name="paramName">
    ///     The parameter name (optional).
    /// </param>
    /// <param name="message">
    ///     The exception message (optional).
    /// </param>
    /// <param name="exceptionCreator">
    ///     The exception creator (optional).
    /// </param>
    /// <returns>
    ///     The current <paramref name="paramValue"/>.
    /// </returns>
    [DebuggerStepThrough]
    public ReadOnlySpan<char> WhiteSpace(ReadOnlySpan<char> paramValue,
        [CallerArgumentExpression(nameof(paramValue))]
        string? paramName = null,
        string? message = null,
        Func<Exception>? exceptionCreator = null) {
        if (!paramValue.IsWhiteSpace()) {
            return paramValue;
        }

        throw exceptionCreator?.Invoke()
              ?? new ArgumentException(string.IsNullOrWhiteSpace(message)
                      ? ARG_WHITE_SPACES_MESSAGE
                      : message,
                  paramName);
    }

    /// <summary>
    ///     Ensure that the <paramref name="paramValue"/> is not
    ///     lower or equal to the value of <paramref name="compare"/>.
    /// </summary>
    /// <param name="paramValue">
    ///     The parameter value.
    /// </param>
    /// <param name="compare">
    ///     The value to compare.
    /// </param>
    /// <param name="paramName">
    ///     The parameter name (optional).
    /// </param>
    /// <param name="message">
    ///     The exception message (optional).
    /// </param>
    /// <param name="exceptionCreator">
    ///     The exception creator (optional).
    /// </param>
    /// <returns>
    ///     The current <paramref name="paramValue"/>.
    /// </returns>
    [DebuggerStepThrough]
    public TimeSpan LowerOrEqual(TimeSpan paramValue,
        TimeSpan compare,
        [CallerArgumentExpression(nameof(paramValue))]
        string? paramName = null,
        string? message = null,
        Func<Exception>? exceptionCreator = null) {
        if (paramValue > compare) {
            return paramValue;
        }

        throw exceptionCreator?.Invoke()
              ?? new ArgumentOutOfRangeException(
                  paramName,
                  paramValue,
                  string.IsNullOrWhiteSpace(message)
                      ? string.Format(ARG_LOWER_OR_EQUAL_MESSAGE, compare)
                      : message);
    }

    /// <summary>
    ///     Ensure that the <paramref name="paramValue"/> is not
    ///     greater or equal to the value of <paramref name="compare"/>.
    /// </summary>
    /// <param name="paramValue">
    ///     The parameter value.
    /// </param>
    /// <param name="compare">
    ///     The value to compare.
    /// </param>
    /// <param name="paramName">
    ///     The parameter name (optional).
    /// </param>
    /// <param name="message">
    ///     The exception message (optional).
    /// </param>
    /// <param name="exceptionCreator">
    ///     The exception creator (optional).
    /// </param>
    /// <returns>
    ///     The current <paramref name="paramValue"/>.
    /// </returns>
    [DebuggerStepThrough]
    public TimeSpan GreaterOrEqual(TimeSpan paramValue,
        TimeSpan compare,
        [CallerArgumentExpression(nameof(paramValue))]
        string? paramName = null,
        string? message = null,
        Func<Exception>? exceptionCreator = null) {
        if (paramValue < compare) {
            return paramValue;
        }

        throw exceptionCreator?.Invoke() ?? new ArgumentOutOfRangeException(
            message: string.IsNullOrWhiteSpace(message)
                ? string.Format(ARG_GREATER_OR_EQUAL_MESSAGE, compare)
                : message,
            paramName: paramName,
            actualValue: paramValue);

    }

    /// <summary>
    ///     Ensure that the <paramref name="paramValue"/> is not
    ///     lower or equal to the value of <paramref name="compare"/>.
    /// </summary>
    /// <param name="paramValue">
    ///     The parameter value.
    /// </param>
    /// <param name="compare">
    ///     The value to compare.
    /// </param>
    /// <param name="paramName">
    ///     The parameter name (optional).
    /// </param>
    /// <param name="message">
    ///     The exception message (optional).
    /// </param>
    /// <param name="exceptionCreator">
    ///     The exception creator (optional).
    /// </param>
    /// <returns>
    ///     The current <paramref name="paramValue"/>.
    /// </returns>
    [DebuggerStepThrough]
    public int LowerOrEqual(int paramValue,
        int compare,
        [CallerArgumentExpression(nameof(paramValue))]
        string? paramName = null,
        string? message = null,
        Func<Exception>? exceptionCreator = null) {
        if (paramValue > compare) {
            return paramValue;
        }

        throw exceptionCreator?.Invoke()
              ?? new ArgumentOutOfRangeException(
                  paramName,
                  paramValue,
                  string.IsNullOrWhiteSpace(message)
                      ? string.Format(ARG_LOWER_OR_EQUAL_MESSAGE, compare)
                      : message);
    }

    /// <summary>
    ///     Ensure that the <paramref name="paramValue"/> is not
    ///     greater or equal to the value of <paramref name="compare"/>.
    /// </summary>
    /// <param name="paramValue">
    ///     The parameter value.
    /// </param>
    /// <param name="compare">
    ///     The value to compare.
    /// </param>
    /// <param name="paramName">
    ///     The parameter name (optional).
    /// </param>
    /// <param name="message">
    ///     The exception message (optional).
    /// </param>
    /// <param name="exceptionCreator">
    ///     The exception creator (optional).
    /// </param>
    /// <returns>
    ///     The current <paramref name="paramValue"/>.
    /// </returns>
    [DebuggerStepThrough]
    public int GreaterOrEqual(int paramValue,
        int compare,
        [CallerArgumentExpression(nameof(paramValue))]
        string? paramName = null,
        string? message = null,
        Func<Exception>? exceptionCreator = null) {
        if (paramValue < compare) {
            return paramValue;
        }

        throw exceptionCreator?.Invoke()
              ?? new ArgumentOutOfRangeException(
                  paramName,
                  paramValue,
                  string.IsNullOrWhiteSpace(message)
                      ? string.Format(ARG_GREATER_OR_EQUAL_MESSAGE, compare)
                      : message);
    }

    /// <summary>
    ///     Ensure that the <paramref name="paramValue"/> is not
    ///     lower than the value of <paramref name="minValue"/>.
    /// </summary>
    /// <param name="paramValue">
    ///     The parameter value.
    /// </param>
    /// <param name="minValue">
    ///     The value to compare.
    /// </param>
    /// <param name="paramName">
    ///     The parameter name (optional).
    /// </param>
    /// <param name="message">
    ///     The exception message (optional).
    /// </param>
    /// <param name="exceptionCreator">
    ///     The exception creator (optional).
    /// </param>
    /// <returns>
    ///     The current <paramref name="paramValue"/>.
    /// </returns>
    [DebuggerStepThrough]
    public int LowerThan(int paramValue,
        int minValue,
        [CallerArgumentExpression(nameof(paramValue))]
        string? paramName = null,
        string? message = null,
        Func<Exception>? exceptionCreator = null) {
        if (paramValue >= minValue) {
            return paramValue;
        }

        throw exceptionCreator?.Invoke()
              ?? new ArgumentOutOfRangeException(
                  paramName,
                  paramValue,
                  string.IsNullOrWhiteSpace(message)
                      ? string.Format(ARG_LOWER_THAN_MESSAGE, minValue)
                      : message);
    }

    /// <summary>
    ///     Ensure that the <paramref name="paramValue"/> is not
    ///     greater than the value of <paramref name="maxValue"/>.
    /// </summary>
    /// <param name="paramValue">
    ///     The parameter value.
    /// </param>
    /// <param name="maxValue">
    ///     The value to compare.
    /// </param>
    /// <param name="paramName">
    ///     The parameter name (optional).
    /// </param>
    /// <param name="message">
    ///     The exception message (optional).
    /// </param>
    /// <param name="exceptionCreator">
    ///     The exception creator (optional).
    /// </param>
    /// <returns>
    ///     The current <paramref name="paramValue"/>.
    /// </returns>
    [DebuggerStepThrough]
    public int GreaterThan(int paramValue,
        int maxValue,
        [CallerArgumentExpression(nameof(paramValue))]
        string? paramName = null,
        string? message = null,
        Func<Exception>? exceptionCreator = null) {
        if (paramValue <= maxValue) {
            return paramValue;
        }

        throw exceptionCreator?.Invoke()
              ?? new ArgumentOutOfRangeException(
                  paramName,
                  paramValue,
                  string.IsNullOrWhiteSpace(message)
                      ? string.Format(ARG_GREATER_THAN_MESSAGE, maxValue)
                      : message);

    }

    /// <summary>
    ///     Ensure that the <paramref name="paramValue"/> is between
    ///     the <paramref name="min"/> and <paramref name="max"/>.
    /// </summary>
    /// <param name="paramValue">
    ///     The parameter value.
    /// </param>
    /// <param name="min">
    ///     The minimum value to compare.
    /// </param>
    /// <param name="max">
    ///     The maximum value to compare.
    /// </param>
    /// <param name="paramName">
    ///     The parameter name (optional).
    /// </param>
    /// <param name="message">
    ///     The exception message (optional).
    /// </param>
    /// <param name="exceptionCreator">
    ///     The exception creator (optional).
    /// </param>
    /// <returns>
    ///     The current <paramref name="paramValue"/>.
    /// </returns>
    [DebuggerStepThrough]
    public int OutOfRange(int paramValue,
        int min,
        int max,
        [CallerArgumentExpression(nameof(paramValue))]
        string? paramName = null,
        string? message = null,
        Func<Exception>? exceptionCreator = null) {
        if (paramValue >= min && paramValue <= max) {
            return paramValue;
        }

        throw exceptionCreator?.Invoke()
              ?? new ArgumentOutOfRangeException(
                  paramName,
                  paramValue,
                  string.IsNullOrWhiteSpace(message)
                      ? string.Format(ARG_OUT_OF_RANGE_MESSAGE, min, max)
                      : message);
    }

    /// <summary>
    ///     Ensure that the <paramref name="paramValue"/> is exactly
    ///     zero.
    /// </summary>
    /// <param name="paramValue">
    ///     The parameter value.
    /// </param>
    /// <param name="paramName">
    ///     The parameter name (optional).
    /// </param>
    /// <param name="message">
    ///     The exception message (optional).
    /// </param>
    /// <param name="exceptionCreator">
    ///     The exception creator (optional).
    /// </param>
    /// <returns>
    ///     The current <paramref name="paramValue"/>.
    /// </returns>
    [DebuggerStepThrough]
    public int Zero(int paramValue,
        [CallerArgumentExpression(nameof(paramValue))]
        string? paramName = null,
        string? message = null,
        Func<Exception>? exceptionCreator = null) {
        if (paramValue > 0) {
            return paramValue;
        }

        throw exceptionCreator?.Invoke()
              ?? new ArgumentException(string.IsNullOrWhiteSpace(message)
                      ? ARG_ZERO_MESSAGE
                      : message,
                  paramName);
    }

    /// <summary>
    ///     Ensure that the <paramref name="paramValue"/> is exactly
    ///     zero.
    /// </summary>
    /// <param name="paramValue">
    ///     The parameter value.
    /// </param>
    /// <param name="paramName">
    ///     The parameter name (optional).
    /// </param>
    /// <param name="message">
    ///     The exception message (optional).
    /// </param>
    /// <param name="exceptionCreator">
    ///     The exception creator (optional).
    /// </param>
    /// <returns>
    ///     The current <paramref name="paramValue"/>.
    /// </returns>
    [DebuggerStepThrough]
    public double Zero(double paramValue,
        [CallerArgumentExpression(nameof(paramValue))]
        string? paramName = null,
        string? message = null,
        Func<Exception>? exceptionCreator = null) {
        if (paramValue > 0D) {
            return paramValue;
        }

        throw exceptionCreator?.Invoke()
              ?? new ArgumentException(string.IsNullOrWhiteSpace(message)
                      ? ARG_ZERO_MESSAGE
                      : message,
                  paramName);
    }

    /// <summary>
    ///     Ensure that the <paramref name="paramValue"/> is exactly
    ///     zero.
    /// </summary>
    /// <param name="paramValue">
    ///     The parameter value.
    /// </param>
    /// <param name="paramName">
    ///     The parameter name (optional).
    /// </param>
    /// <param name="message">
    ///     The exception message (optional).
    /// </param>
    /// <param name="exceptionCreator">
    ///     The exception creator (optional).
    /// </param>
    /// <returns>
    ///     The current <paramref name="paramValue"/>.
    /// </returns>
    [DebuggerStepThrough]
    public decimal Zero(decimal paramValue,
        [CallerArgumentExpression(nameof(paramValue))]
        string? paramName = null,
        string? message = null,
        Func<Exception>? exceptionCreator = null) {
        if (paramValue > 0M) {
            return paramValue;
        }

        throw exceptionCreator?.Invoke()
              ?? new ArgumentException(string.IsNullOrWhiteSpace(message)
                      ? ARG_ZERO_MESSAGE
                      : message,
                  paramName);
    }

    /// <summary>
    ///     Ensure that the <paramref name="paramValue"/> is exactly
    ///     zero.
    /// </summary>
    /// <param name="paramValue">
    ///     The parameter value.
    /// </param>
    /// <param name="paramName">
    ///     The parameter name (optional).
    /// </param>
    /// <param name="message">
    ///     The exception message (optional).
    /// </param>
    /// <param name="exceptionCreator">
    ///     The exception creator (optional).
    /// </param>
    /// <returns>
    ///     The current <paramref name="paramValue"/>.
    /// </returns>
    [DebuggerStepThrough]
    public TimeSpan Zero(TimeSpan paramValue,
        [CallerArgumentExpression(nameof(paramValue))]
        string? paramName = null,
        string? message = null,
        Func<Exception>? exceptionCreator = null) {
        if (paramValue > TimeSpan.Zero) {
            return paramValue;
        }

        throw exceptionCreator?.Invoke()
              ?? new ArgumentException(string.IsNullOrWhiteSpace(message)
                      ? ARG_ZERO_MESSAGE
                      : message,
                  paramName);
    }

    /// <summary>
    ///     Ensure that the <paramref name="paramValue"/> is
    ///     not a negative value.
    /// </summary>
    /// <param name="paramValue">
    ///     The parameter value.
    /// </param>
    /// <param name="paramName">
    ///     The parameter name (optional).
    /// </param>
    /// <param name="message">
    ///     The exception message (optional).
    /// </param>
    /// <param name="exceptionCreator">
    ///     The exception creator (optional).
    /// </param>
    /// <returns>
    ///     The current <paramref name="paramValue"/>.
    /// </returns>
    [DebuggerStepThrough]
    public TimeSpan Negative(TimeSpan paramValue,
        [CallerArgumentExpression(nameof(paramValue))]
        string? paramName = null,
        string? message = null,
        Func<Exception>? exceptionCreator = null) {
        if (paramValue >= TimeSpan.Zero) {
            return paramValue;
        }

        throw exceptionCreator?.Invoke()
              ?? new ArgumentException(string.IsNullOrWhiteSpace(message)
                      ? ARG_ZERO_MESSAGE
                      : message,
                  paramName);
    }

    /// <summary>
    ///     Ensure that the <paramref name="paramValue"/> is defined
    ///     inside the <see cref="Enum"/> type specified by
    ///     <typeparamref name="TEnum"/>.
    /// </summary>
    /// <param name="paramValue">
    ///     The parameter value.
    /// </param>
    /// <param name="paramName">
    ///     The parameter name (optional).
    /// </param>
    /// <param name="message">
    ///     The exception message.
    /// </param>
    /// <param name="exceptionCreator">
    ///     The exception creator (optional).
    /// </param>
    /// <returns>
    ///     The current <paramref name="paramValue"/>.
    /// </returns>
    [DebuggerStepThrough]
    public TEnum UndefinedEnum<TEnum>(int paramValue,
        [CallerArgumentExpression(nameof(paramValue))]
        string? paramName = null,
        string? message = null,
        Func<Exception>? exceptionCreator = null)
        where TEnum : struct, Enum {
        if (Enum.IsDefined(typeof(TEnum), paramValue)) {
            return (TEnum)Enum.ToObject(typeof(TEnum), paramValue);
        }

        throw exceptionCreator?.Invoke()
              ?? new InvalidEnumArgumentException(
                  string.IsNullOrWhiteSpace(message)
                      ? string.Format(ARG_UNDEFINED_ENUM_MESSAGE, paramName, paramValue, typeof(TEnum))
                      : message);
    }

    /// <summary>
    ///     Ensure that the <paramref name="paramValue"/> is defined
    ///     inside the <see cref="Enum"/> type specified by
    ///     <typeparamref name="TEnum"/>.
    /// </summary>
    /// <param name="paramValue">
    ///     The parameter value.
    /// </param>
    /// <param name="paramName">
    ///     The parameter name (optional).
    /// </param>
    /// <param name="message">
    ///     The exception message.
    /// </param>
    /// <param name="exceptionCreator">
    ///     The exception creator (optional).
    /// </param>
    /// <returns>
    ///     The current <paramref name="paramValue"/>.
    /// </returns>
    [DebuggerStepThrough]
    public TEnum UndefinedEnum<TEnum>(string paramValue,
        [CallerArgumentExpression(nameof(paramValue))]
        string? paramName = null,
        string? message = null,
        Func<Exception>? exceptionCreator = null)
        where TEnum : struct, Enum {
        if (Enum.IsDefined(typeof(TEnum), paramValue)) {
            return (TEnum)Enum.ToObject(typeof(TEnum), paramValue);
        }

        throw exceptionCreator?.Invoke()
              ?? new InvalidEnumArgumentException(
                  string.IsNullOrWhiteSpace(message)
                      ? string.Format(ARG_UNDEFINED_ENUM_MESSAGE, paramName, paramValue, typeof(TEnum))
                      : message);
    }

    /// <summary>
    ///     Ensure that the <paramref name="paramValue"/> is assignable
    ///     from the type defined by <typeparamref name="TType"/>.
    /// </summary>
    /// <typeparam name="TType">
    ///     Type to check against.
    /// </typeparam>
    /// <param name="paramValue">
    ///     The parameter value.
    /// </param>
    /// <param name="paramName">
    ///     The parameter name (optional).
    /// </param>
    /// <param name="message">
    ///     The exception message.
    /// </param>
    /// <param name="exceptionCreator">
    ///     The exception creator (optional).
    /// </param>
    /// <returns>
    ///     The current <paramref name="paramValue"/>.
    /// </returns>
    [DebuggerStepThrough]
    public Type NotAssignableFrom<TType>(Type paramValue,
        [CallerArgumentExpression(nameof(paramValue))]
        string? paramName = null,
        string? message = null,
        Func<Exception>? exceptionCreator = null) {
        return NotAssignableFrom(paramValue, typeof(TType), paramName, message, exceptionCreator);
    }

    /// <summary>
    ///     Ensure that the <paramref name="paramValue"/> is assignable
    ///     from the type defined by <paramref name="type"/>.
    /// </summary>
    /// <param name="paramValue">
    ///     The parameter value.
    /// </param>
    /// <param name="type">
    ///     The type to compare.
    /// </param>
    /// <param name="paramName">
    ///     The parameter name (optional).
    /// </param>
    /// <param name="message">
    ///     The exception message.
    /// </param>
    /// <param name="exceptionCreator">
    ///     The exception creator (optional).
    /// </param>
    /// <returns>
    ///     The current <paramref name="paramValue"/>.
    /// </returns>
    [DebuggerStepThrough]
    public Type NotAssignableFrom(Type paramValue,
        Type type,
        [CallerArgumentExpression(nameof(paramValue))]
        string? paramName = null,
        string? message = null,
        Func<Exception>? exceptionCreator = null) {
        Null(paramValue);
        Null(type);

        if (type.IsAssignableFrom(paramValue)) {
            return paramValue;
        }

        throw exceptionCreator?.Invoke()
              ?? new ArgumentException(string.IsNullOrWhiteSpace(message)
                  ? string.Format(ARG_NOT_ASSIGNABLE_FROM_MESSAGE, paramName, type, paramValue)
                  : message);
    }
}