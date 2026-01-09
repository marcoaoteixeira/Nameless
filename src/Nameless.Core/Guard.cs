#pragma warning disable CA1822

using System.Collections;
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
public sealed class Guard {
    private const string PARAM_NULL_MESSAGE = "Parameter cannot be null.";
    private const string PARAM_EMPTY_MESSAGE = "Parameter cannot be empty.";
    private const string PARAM_DEFAULT_MESSAGE = "Parameter cannot be default.";
    private const string PARAM_WHITE_SPACES_MESSAGE = "Parameter cannot be white spaces.";
    private const string PARAM_NO_MATCHING_PATTERN_MESSAGE = "Parameter does not match pattern: {0}";
    private const string PARAM_LOWER_OR_EQUAL_MESSAGE = "Parameter cannot be lower or equal to '{0}'.";
    private const string PARAM_GREATER_OR_EQUAL_MESSAGE = "Parameter cannot be greater or equal to '{0}'.";
    private const string PARAM_LOWER_THAN_MESSAGE = "Parameter cannot be lower than '{0}'.";
    private const string PARAM_GREATER_THAN_MESSAGE = "Parameter cannot be greater than '{0}'.";
    private const string PARAM_OUT_OF_RANGE_MESSAGE = "Parameter must be between minimum value of '{0}' and maximum value of '{1}'.";
    private const string PARAM_ZERO_MESSAGE = "Parameter cannot be zero value.";
    private const string PARAM_NOT_ASSIGNABLE_FROM_MESSAGE = "Parameter '{0}' of type '{1}' is not assignable to '{2}'.";

    /// <summary>
    ///     Gets the unique instance of <see cref="Guard" />.
    /// </summary>
    public static Guard Against { get; } = new();

    // Explicit static constructor to tell the C# compiler
    // not to mark type as beforefieldinit
    static Guard() { }

    private Guard() { }

    /// <summary>
    ///     Ensure that the <paramref name="paramValue"/> is
    ///     not <see langword="null"/>.
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
    /// <remarks>
    ///     The <paramref name="paramValue"/> must be a reference type.
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    ///     If <paramref name="paramValue"/> is <see langword="null"/>.
    /// </exception>
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
                  ? PARAM_NULL_MESSAGE
                  : message);
    }

    /// <summary>
    ///     Ensure that the <paramref name="paramValue"/> is
    ///     not <see langword="null"/>.
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
    /// <remarks>
    ///     The <paramref name="paramValue"/> must be a value type.
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    ///     If <paramref name="paramValue"/> is <see langword="null"/>.
    /// </exception>
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
                  ? PARAM_NULL_MESSAGE
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
    /// <exception cref="ArgumentNullException">
    ///     If <paramref name="paramValue"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     If <paramref name="paramValue"/> is empty.
    /// </exception>
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
                      ? PARAM_EMPTY_MESSAGE
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
    /// <exception cref="ArgumentNullException">
    ///     If <paramref name="paramValue"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     If <paramref name="paramValue"/> is empty or only whitespace.
    /// </exception>
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
                      ? PARAM_WHITE_SPACES_MESSAGE
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
    /// <exception cref="ArgumentNullException">
    ///     If <paramref name="paramValue"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     If <paramref name="paramValue"/> is empty.
    /// </exception>
    [DebuggerStepThrough]
    public Guid NullOrEmpty([NotNull] Guid? paramValue,
        [CallerArgumentExpression(nameof(paramValue))]
        string? paramName = null,
        string? message = null,
        Func<Exception>? exceptionCreator = null) {
        Null(paramValue, paramName, message, exceptionCreator);

        if (paramValue == Guid.Empty) {
            throw exceptionCreator?.Invoke() ?? new ArgumentException(string.IsNullOrWhiteSpace(message)
                    ? PARAM_EMPTY_MESSAGE
                    : message,
                paramName);
        }

        return paramValue.Value;
    }

    /// <summary>
    ///     Ensure that the <paramref name="paramValue"/> is not
    ///     <see langword="null"/> or empty.
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
    /// <remarks>
    ///     The <paramref name="paramValue"/> must implements
    ///     <see cref="IEnumerable"/>.
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    ///     If <paramref name="paramValue"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     If <paramref name="paramValue"/> is empty.
    /// </exception>
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
                          ? PARAM_EMPTY_MESSAGE
                          : message,
                      paramName);
        }
    }

    /// <summary>
    ///     Ensure that the <paramref name="paramValue"/> is not
    ///     <see langword="default"/>.
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
    /// <exception cref="ArgumentException">
    ///     If <paramref name="paramValue"/> is <see langword="default"/>.
    /// </exception>
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
                      ? PARAM_DEFAULT_MESSAGE
                      : message,
                  paramName);
    }

    /// <summary>
    ///     Ensure that the <paramref name="paramValue"/> matches
    ///     the specified <paramref name="regexPattern"/>.
    /// </summary>
    /// <param name="paramValue">
    ///     The parameter value.
    /// </param>
    /// <param name="regexPattern">
    ///     The regex pattern to match.
    /// </param>
    /// <param name="ignoreCase">
    ///     Whether it should ignore the case when matching.
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
    /// <exception cref="ArgumentNullException">
    ///     If <paramref name="paramValue"/> or
    ///     <paramref name="regexPattern"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     If a regular expression parsing error occurred, or
    ///     if the <paramref name="paramValue"/> do not match
    ///     the <paramref name="regexPattern"/>.
    /// </exception>
    /// <exception cref="RegexMatchTimeoutException">
    ///     If a time-out occurred.
    /// </exception>
    [DebuggerStepThrough]
    public string NoMatchingPattern(string paramValue,
        string regexPattern,
        bool ignoreCase = false,
        [CallerArgumentExpression(nameof(paramValue))]
        string? paramName = null,
        string? message = null,
        Func<Exception>? exceptionCreator = null) {
        var options = ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None;
        var comparison = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
        var match = Regex.Match(paramValue, regexPattern, options);
        if (match.Success && string.Equals(match.Value, paramValue, comparison)) {
            return paramValue;
        }

        throw exceptionCreator?.Invoke()
              ?? new ArgumentException(string.IsNullOrWhiteSpace(message)
                      ? string.Format(PARAM_NO_MATCHING_PATTERN_MESSAGE, regexPattern)
                      : message,
                  paramName);
    }

    /// <summary>
    ///     Ensure that the <paramref name="paramValue"/> matches
    ///     the specified <paramref name="regex"/>.
    /// </summary>
    /// <param name="paramValue">
    ///     The parameter value.
    /// </param>
    /// <param name="regex">
    ///     The regex object.
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
    /// <exception cref="ArgumentNullException">
    ///     If <paramref name="paramValue"/> or
    ///     <paramref name="regex"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     If a regular expression parsing error occurred, or
    ///     if the <paramref name="paramValue"/> do not match
    ///     the <paramref name="regex"/>.
    /// </exception>
    /// <exception cref="RegexMatchTimeoutException">
    ///     If a time-out occurred.
    /// </exception>
    [DebuggerStepThrough]
    public string NoMatchingPattern(string paramValue,
        Regex regex,
        [CallerArgumentExpression(nameof(paramValue))]
        string? paramName = null,
        string? message = null,
        Func<Exception>? exceptionCreator = null) {
        Null(regex);

        var comparison = regex.Options.HasFlag(RegexOptions.IgnoreCase)
            ? StringComparison.OrdinalIgnoreCase
            : StringComparison.Ordinal;

        var match = regex.Match(paramValue);
        if (match.Success && string.Equals(match.Value, paramValue, comparison)) {
            return paramValue;
        }

        throw exceptionCreator?.Invoke()
              ?? new ArgumentException(string.IsNullOrWhiteSpace(message)
                      ? string.Format(PARAM_NO_MATCHING_PATTERN_MESSAGE, regex)
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
    /// <exception cref="ArgumentException">
    ///     If <paramref name="paramValue"/> is empty.
    /// </exception>
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
                      ? PARAM_EMPTY_MESSAGE
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
    /// <exception cref="ArgumentException">
    ///     If <paramref name="paramValue"/> is only whitespace.
    /// </exception>
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
                      ? PARAM_WHITE_SPACES_MESSAGE
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
    /// <exception cref="ArgumentOutOfRangeException">
    ///     If <paramref name="paramValue"/> is lower or equal
    ///     to <paramref name="compare"/>.
    /// </exception>
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
                      ? string.Format(PARAM_LOWER_OR_EQUAL_MESSAGE, compare)
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
    /// <exception cref="ArgumentOutOfRangeException">
    ///     If <paramref name="paramValue"/> is greater or equal
    ///     to <paramref name="compare"/>.
    /// </exception>
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
                ? string.Format(PARAM_GREATER_OR_EQUAL_MESSAGE, compare)
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
    /// <exception cref="ArgumentOutOfRangeException">
    ///     If <paramref name="paramValue"/> is lower or equal
    ///     to <paramref name="compare"/>.
    /// </exception>
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
                      ? string.Format(PARAM_LOWER_OR_EQUAL_MESSAGE, compare)
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
    /// <exception cref="ArgumentOutOfRangeException">
    ///     If <paramref name="paramValue"/> is greater or equal
    ///     to <paramref name="compare"/>.
    /// </exception>
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
                      ? string.Format(PARAM_GREATER_OR_EQUAL_MESSAGE, compare)
                      : message);
    }

    /// <summary>
    ///     Ensure that the <paramref name="paramValue"/> is not
    ///     lower than the value of <paramref name="compare"/>.
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
    /// <exception cref="ArgumentOutOfRangeException">
    ///     If <paramref name="paramValue"/> is lower than
    ///     <paramref name="compare"/>.
    /// </exception>
    [DebuggerStepThrough]
    public int LowerThan(int paramValue,
        int compare,
        [CallerArgumentExpression(nameof(paramValue))]
        string? paramName = null,
        string? message = null,
        Func<Exception>? exceptionCreator = null) {
        if (paramValue >= compare) {
            return paramValue;
        }

        throw exceptionCreator?.Invoke()
              ?? new ArgumentOutOfRangeException(
                  paramName,
                  paramValue,
                  string.IsNullOrWhiteSpace(message)
                      ? string.Format(PARAM_LOWER_THAN_MESSAGE, compare)
                      : message);
    }

    /// <summary>
    ///     Ensure that the <paramref name="paramValue"/> is not
    ///     greater than the value of <paramref name="compare"/>.
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
    /// <exception cref="ArgumentOutOfRangeException">
    ///     If <paramref name="paramValue"/> is greater than
    ///     <paramref name="compare"/>.
    /// </exception>
    [DebuggerStepThrough]
    public int GreaterThan(int paramValue,
        int compare,
        [CallerArgumentExpression(nameof(paramValue))]
        string? paramName = null,
        string? message = null,
        Func<Exception>? exceptionCreator = null) {
        if (paramValue <= compare) {
            return paramValue;
        }

        throw exceptionCreator?.Invoke()
              ?? new ArgumentOutOfRangeException(
                  paramName,
                  paramValue,
                  string.IsNullOrWhiteSpace(message)
                      ? string.Format(PARAM_GREATER_THAN_MESSAGE, compare)
                      : message);
    }

    /// <summary>
    ///     Ensure that the <paramref name="paramValue"/> is between
    ///     the <paramref name="minimumValue"/> and
    ///     <paramref name="maximumValue"/>.
    /// </summary>
    /// <param name="paramValue">
    ///     The parameter value.
    /// </param>
    /// <param name="minimumValue">
    ///     The minimum value to compare.
    /// </param>
    /// <param name="maximumValue">
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
    /// <exception cref="ArgumentOutOfRangeException">
    ///     If <paramref name="paramValue"/> is not between
    ///     <paramref name="minimumValue"/> and <paramref name="maximumValue"/>.
    /// </exception>
    [DebuggerStepThrough]
    public int OutOfRange(int paramValue,
        int minimumValue,
        int maximumValue,
        [CallerArgumentExpression(nameof(paramValue))]
        string? paramName = null,
        string? message = null,
        Func<Exception>? exceptionCreator = null) {
        if (paramValue >= minimumValue && paramValue <= maximumValue) {
            return paramValue;
        }

        throw exceptionCreator?.Invoke()
              ?? new ArgumentOutOfRangeException(
                  paramName,
                  paramValue,
                  string.IsNullOrWhiteSpace(message)
                      ? string.Format(PARAM_OUT_OF_RANGE_MESSAGE, minimumValue, maximumValue)
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
    /// <exception cref="ArgumentException">
    ///     If <paramref name="paramValue"/> is zero value.
    /// </exception>
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
                      ? PARAM_ZERO_MESSAGE
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
    /// <exception cref="ArgumentException">
    ///     If <paramref name="paramValue"/> is zero value.
    /// </exception>
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
                      ? PARAM_ZERO_MESSAGE
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
    /// <exception cref="ArgumentException">
    ///     If <paramref name="paramValue"/> is zero value.
    /// </exception>
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
                      ? PARAM_ZERO_MESSAGE
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
    /// <exception cref="ArgumentException">
    ///     If <paramref name="paramValue"/> is zero value.
    /// </exception>
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
                      ? PARAM_ZERO_MESSAGE
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
    /// <exception cref="ArgumentException">
    ///     If <paramref name="paramValue"/> is negative value.
    /// </exception>
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
                      ? PARAM_ZERO_MESSAGE
                      : message,
                  paramName);
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
    /// <exception cref="ArgumentNullException">
    ///     If <paramref name="paramValue"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     If <typeparamref name="TType"/> is not assignable
    ///     from <paramref name="paramValue"/>.
    /// </exception>
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
    /// <exception cref="ArgumentNullException">
    ///     If <paramref name="paramValue"/> or
    ///     <paramref name="type"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     If <paramref name="type"/> is not assignable
    ///     from <paramref name="paramValue"/>.
    /// </exception>
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
                  ? string.Format(PARAM_NOT_ASSIGNABLE_FROM_MESSAGE, paramName, paramValue, type)
                  : message);
    }
}