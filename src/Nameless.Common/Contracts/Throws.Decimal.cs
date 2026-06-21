// ReSharper disable ClassCannotBeInstantiated
#pragma warning disable CA1822

using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Nameless;

public sealed partial class Throws {
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
    public decimal LowerOrEqual(decimal paramValue, decimal compare, [CallerArgumentExpression(nameof(paramValue))] string? paramName = null, string? message = null, Func<Exception>? exceptionCreator = null) {
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
    public decimal GreaterOrEqual(decimal paramValue, decimal compare, [CallerArgumentExpression(nameof(paramValue))] string? paramName = null, string? message = null, Func<Exception>? exceptionCreator = null) {
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
    public decimal LowerThan(decimal paramValue, decimal compare, [CallerArgumentExpression(nameof(paramValue))] string? paramName = null, string? message = null, Func<Exception>? exceptionCreator = null) {
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
    public decimal GreaterThan(decimal paramValue, decimal compare, [CallerArgumentExpression(nameof(paramValue))] string? paramName = null, string? message = null, Func<Exception>? exceptionCreator = null) {
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
    public decimal OutOfRange(decimal paramValue, decimal minimumValue, decimal maximumValue, [CallerArgumentExpression(nameof(paramValue))] string? paramName = null, string? message = null, Func<Exception>? exceptionCreator = null) {
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
    public decimal Zero(decimal paramValue, [CallerArgumentExpression(nameof(paramValue))] string? paramName = null, string? message = null, Func<Exception>? exceptionCreator = null) {
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
    public decimal Negative(decimal paramValue, [CallerArgumentExpression(nameof(paramValue))] string? paramName = null, string? message = null, Func<Exception>? exceptionCreator = null) {
        if (!decimal.IsNegative(paramValue)) {
            return paramValue;
        }

        throw exceptionCreator?.Invoke()
              ?? new ArgumentException(string.IsNullOrWhiteSpace(message)
                      ? PARAM_NEGATIVE_MESSAGE
                      : message,
                  paramName);
    }
}
