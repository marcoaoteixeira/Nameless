// ReSharper disable ClassCannotBeInstantiated
#pragma warning disable CA1822

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Nameless;

public sealed partial class Throws {
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
    public string NullOrEmpty([NotNull] string? paramValue, [CallerArgumentExpression(nameof(paramValue))] string? paramName = null, string? message = null, Func<Exception>? exceptionCreator = null) {
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
    public string NullOrWhiteSpace([NotNull] string? paramValue, [CallerArgumentExpression(nameof(paramValue))] string? paramName = null, string? message = null, Func<Exception>? exceptionCreator = null) {
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
    public string NoMatchingPattern(string paramValue, string regexPattern, bool ignoreCase = false, [CallerArgumentExpression(nameof(paramValue))] string? paramName = null, string? message = null, Func<Exception>? exceptionCreator = null) {
        Null(paramValue, paramName, message, exceptionCreator);

        var options = ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None;

        if (Regex.IsMatch(paramValue, regexPattern, options)) {
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
    public string NoMatchingPattern(string paramValue, Regex regex, [CallerArgumentExpression(nameof(paramValue))] string? paramName = null, string? message = null, Func<Exception>? exceptionCreator = null) {
        Null(paramValue, paramName, message, exceptionCreator);

        if (regex.IsMatch(paramValue)) {
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
    public ReadOnlySpan<char> Empty(ReadOnlySpan<char> paramValue, [CallerArgumentExpression(nameof(paramValue))] string? paramName = null, string? message = null, Func<Exception>? exceptionCreator = null) {
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
    public ReadOnlySpan<char> WhiteSpace(ReadOnlySpan<char> paramValue, [CallerArgumentExpression(nameof(paramValue))] string? paramName = null, string? message = null, Func<Exception>? exceptionCreator = null) {
        if (!paramValue.IsWhiteSpace()) {
            return paramValue;
        }

        throw exceptionCreator?.Invoke()
              ?? new ArgumentException(string.IsNullOrWhiteSpace(message)
                      ? PARAM_WHITE_SPACES_MESSAGE
                      : message,
                  paramName);
    }
}
