#pragma warning disable CA1822

using System.Collections;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Nameless;

/// <summary>
/// A guard clause (https://en.wikipedia.org/wiki/Guard_(computer_science))
/// is a software pattern that simplifies complex functions by
/// "failing fast", checking for invalid inputs up front and immediately
/// failing if any are found.
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

    /// <summary>
    /// Gets the unique instance of <see cref="Prevent" />.
    /// </summary>
    public static Prevent Argument { get; } = new();

    // Explicit static constructor to tell the C# compiler
    // not to mark type as beforefieldinit
    static Prevent() { }

    private Prevent() { }

    /// <summary>
    /// Throws an <see cref="ArgumentNullException"/> if parameter <paramref name="paramValue"/> is <c>null</c>.
    /// </summary>
    /// <typeparam name="TValue">Type of the parameter value.</typeparam>
    /// <param name="paramValue">The parameter value.</param>
    /// <param name="paramName">The parameter name (optional).</param>
    /// <param name="message">The exception message (optional).</param>
    /// <param name="exceptionCreator">The exception creator (optional).</param>
    /// <returns>
    /// The current <paramref name="paramValue"/>.
    /// </returns>
    [DebuggerStepThrough]
    public TValue Null<TValue>([NotNull] TValue? paramValue,
                               [CallerArgumentExpression(nameof(paramValue))] string? paramName = null,
                               string? message = null,
                               Func<Exception>? exceptionCreator = null) where TValue : class {
        if (paramValue is not null) {
            return paramValue;
        }

        throw exceptionCreator?.Invoke() ?? new ArgumentNullException(paramName: paramName,
                                                                      message: string.IsNullOrWhiteSpace(message)
                                                                          ? ARG_NULL_MESSAGE
                                                                          : message);
    }

    /// <summary>
    /// Throws an <see cref="ArgumentNullException"/> if parameter <paramref name="paramValue"/> is <c>null</c>.
    /// </summary>
    /// <typeparam name="TValue">Type of the parameter value.</typeparam>
    /// <param name="paramValue">The parameter value.</param>
    /// <param name="paramName">The parameter name (optional).</param>
    /// <param name="message">The exception message (optional).</param>
    /// <param name="exceptionCreator">The exception creator (optional).</param>
    /// <returns>
    /// The current <paramref name="paramValue"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="paramValue"/> is <c>null</c>.
    /// </exception>
    [DebuggerStepThrough]
    public TValue Null<TValue>([NotNull] TValue? paramValue,
                               [CallerArgumentExpression(nameof(paramValue))] string? paramName = null,
                               string? message = null,
                               Func<Exception>? exceptionCreator = null) where TValue : struct {
        if (paramValue is not null) {
            return paramValue.Value;
        }

        throw exceptionCreator?.Invoke() ?? new ArgumentNullException(paramName: paramName,
                                                                      message: string.IsNullOrWhiteSpace(message)
                                                                          ? ARG_NULL_MESSAGE
                                                                          : message);
    }

    [DebuggerStepThrough]
    public string NullOrEmpty([NotNull] string? paramValue,
                              [CallerArgumentExpression(nameof(paramValue))] string? paramName = null,
                              string? message = null,
                              Func<Exception>? exceptionCreator = null) {
        Null(paramValue, paramName, message, exceptionCreator);

        if (paramValue.Length == 0) {
            throw exceptionCreator?.Invoke() ?? new ArgumentException(message: string.IsNullOrWhiteSpace(message)
                                                                          ? ARG_EMPTY_MESSAGE
                                                                          : message,
                                                                      paramName: paramName);
        }

        return paramValue;
    }

    [DebuggerStepThrough]
    public string NullOrWhiteSpace([NotNull] string? paramValue,
                                   [CallerArgumentExpression(nameof(paramValue))] string? paramName = null,
                                   string? message = null,
                                   Func<Exception>? exceptionCreator = null) {
        NullOrEmpty(paramValue, paramName, message, exceptionCreator);

        if (paramValue.Trim().Length == 0) {
            throw exceptionCreator?.Invoke() ?? new ArgumentException(message: string.IsNullOrWhiteSpace(message)
                                                                          ? ARG_WHITE_SPACES_MESSAGE
                                                                          : message,
                                                                      paramName: paramName);
        }

        return paramValue;
    }

    [DebuggerStepThrough]
    public Guid NullOrEmpty([NotNull] Guid? paramValue,
                            [CallerArgumentExpression(nameof(paramValue))] string? paramName = null,
                            string? message = null,
                            Func<Exception>? exceptionCreator = null) {
        Null(paramValue, paramName, message, exceptionCreator);

        if (paramValue == Guid.Empty) {
            throw exceptionCreator?.Invoke() ?? new ArgumentException(message: string.IsNullOrWhiteSpace(message)
                                                                          ? ARG_EMPTY_MESSAGE
                                                                          : message,
                                                                      paramName: paramName);
        }

        return paramValue.Value;
    }

    [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
    [DebuggerStepThrough]
    public T NullOrEmpty<T>([NotNull] T? paramValue,
                            [CallerArgumentExpression(nameof(paramValue))] string? paramName = null,
                            string? message = null,
                            Func<Exception>? exceptionCreator = null) where T : class, IEnumerable {

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
            throw exceptionCreator?.Invoke() ?? new ArgumentException(message: string.IsNullOrWhiteSpace(message)
                                                                          ? ARG_EMPTY_MESSAGE
                                                                          : message,
                                                                      paramName: paramName);
        }
    }

    [DebuggerStepThrough]
    public T Default<T>([NotNull] T? paramValue,
                        [CallerArgumentExpression(nameof(paramValue))] string? paramName = null,
                        string? message = null,
                        Func<Exception>? exceptionCreator = null) {
        if (EqualityComparer<T?>.Default.Equals(paramValue, default) || paramValue is null) {
            throw exceptionCreator?.Invoke() ?? new ArgumentException(message: string.IsNullOrWhiteSpace(message)
                                                                          ? ARG_DEFAULT_MESSAGE
                                                                          : message,
                                                                      paramName: paramName);
        }

        return paramValue;
    }

    [DebuggerStepThrough]
    public string NoMatchingPattern(string paramValue,
                                    string pattern,
                                    [CallerArgumentExpression(nameof(paramValue))] string? paramName = null,
                                    string? message = null,
                                    Func<Exception>? exceptionCreator = null) {
        var match = Regex.Match(paramValue, pattern);
        if (!match.Success || match.Value != paramValue) {
            throw exceptionCreator?.Invoke() ?? new ArgumentException(message: string.IsNullOrWhiteSpace(message)
                                                                          ? string.Format(ARG_NO_MATCHING_PATTERN_MESSAGE, pattern)
                                                                          : message,
                                                                      paramName: paramName);
        }

        return paramValue;
    }

    [DebuggerStepThrough]
    public ReadOnlySpan<char> Empty(ReadOnlySpan<char> paramValue,
                                    [CallerArgumentExpression(nameof(paramValue))] string? paramName = null,
                                    string? message = null,
                                    Func<Exception>? exceptionCreator = null) {
        if (paramValue.Length == 0 || paramValue == string.Empty) {
            throw exceptionCreator?.Invoke() ?? new ArgumentException(message: string.IsNullOrWhiteSpace(message)
                                                                          ? ARG_EMPTY_MESSAGE
                                                                          : message,
                                                                      paramName: paramName);
        }

        return paramValue;
    }

    [DebuggerStepThrough]
    public ReadOnlySpan<char> WhiteSpace(ReadOnlySpan<char> paramValue,
                                         [CallerArgumentExpression(nameof(paramValue))] string? paramName = null,
                                         string? message = null,
                                         Func<Exception>? exceptionCreator = null) {
        if (paramValue.IsWhiteSpace()) {
            throw exceptionCreator?.Invoke() ?? new ArgumentException(message: string.IsNullOrWhiteSpace(message)
                                                                          ? ARG_WHITE_SPACES_MESSAGE
                                                                          : message,
                                                                      paramName: paramName);
        }

        return paramValue;
    }

    [DebuggerStepThrough]
    public TimeSpan LowerOrEqual(TimeSpan paramValue,
                                 TimeSpan to,
                                 [CallerArgumentExpression(nameof(paramValue))] string? paramName = null,
                                 string? message = null,
                                 Func<Exception>? exceptionCreator = null) {
        if (paramValue <= to) {
            throw exceptionCreator?.Invoke() ?? new ArgumentOutOfRangeException(message: string.IsNullOrWhiteSpace(message)
                                                                                    ? string.Format(ARG_LOWER_OR_EQUAL_MESSAGE, to)
                                                                                    : message,
                                                                                paramName: paramName,
                                                                                actualValue: paramValue);
        }

        return paramValue;
    }

    [DebuggerStepThrough]
    public TimeSpan GreaterOrEqual(TimeSpan paramValue,
                                   TimeSpan to,
                                   [CallerArgumentExpression(nameof(paramValue))] string? paramName = null,
                                   string? message = null,
                                   Func<Exception>? exceptionCreator = null) {
        if (paramValue >= to) {
            throw exceptionCreator?.Invoke() ?? new ArgumentOutOfRangeException(message: string.IsNullOrWhiteSpace(message)
                                                                                    ? string.Format(ARG_GREATER_OR_EQUAL_MESSAGE, to)
                                                                                    : message,
                                                                                paramName: paramName,
                                                                                actualValue: paramValue);
        }

        return paramValue;
    }

    [DebuggerStepThrough]
    public int LowerOrEqual(int paramValue,
                            int to,
                            [CallerArgumentExpression(nameof(paramValue))] string? paramName = null,
                            string? message = null,
                            Func<Exception>? exceptionCreator = null) {
        if (paramValue <= to) {
            throw exceptionCreator?.Invoke() ?? new ArgumentOutOfRangeException(message: string.IsNullOrWhiteSpace(message)
                                                                                    ? string.Format(ARG_LOWER_OR_EQUAL_MESSAGE, to)
                                                                                    : message,
                                                                                paramName: paramName,
                                                                                actualValue: paramValue);
        }

        return paramValue;
    }

    [DebuggerStepThrough]
    public int GreaterOrEqual(int paramValue,
                              int to,
                              [CallerArgumentExpression(nameof(paramValue))] string? paramName = null,
                              string? message = null,
                              Func<Exception>? exceptionCreator = null) {
        if (paramValue >= to) {
            throw exceptionCreator?.Invoke() ?? new ArgumentOutOfRangeException(message: string.IsNullOrWhiteSpace(message)
                                                                                    ? string.Format(ARG_GREATER_OR_EQUAL_MESSAGE, to)
                                                                                    : message,
                                                                                paramName: paramName,
                                                                                actualValue: paramValue);
        }

        return paramValue;
    }

    [DebuggerStepThrough]
    public int LowerThan(int paramValue,
                         int minValue,
                         [CallerArgumentExpression(nameof(paramValue))] string? paramName = null,
                         string? message = null,
                         Func<Exception>? exceptionCreator = null) {
        if (paramValue < minValue) {
            throw exceptionCreator?.Invoke() ?? new ArgumentOutOfRangeException(message: string.IsNullOrWhiteSpace(message)
                                                                                    ? string.Format(ARG_LOWER_THAN_MESSAGE, minValue)
                                                                                    : message,
                                                                                paramName: paramName,
                                                                                actualValue: paramValue);
        }

        return paramValue;
    }

    [DebuggerStepThrough]
    public int GreaterThan(int paramValue,
                           int maxValue,
                           [CallerArgumentExpression(nameof(paramValue))] string? paramName = null,
                           string? message = null,
                           Func<Exception>? exceptionCreator = null) {
        if (paramValue > maxValue) {
            throw exceptionCreator?.Invoke() ?? new ArgumentOutOfRangeException(message: string.IsNullOrWhiteSpace(message)
                                                                                    ? string.Format(ARG_GREATER_THAN_MESSAGE, maxValue)
                                                                                    : message,
                                                                                paramName: paramName,
                                                                                actualValue: paramValue);
        }

        return paramValue;
    }

    [DebuggerStepThrough]
    public int OutOfRange(int paramValue,
                          int min,
                          int max,
                          [CallerArgumentExpression(nameof(paramValue))] string? paramName = null,
                          string? message = null,
                          Func<Exception>? exceptionCreator = null) {
        if (paramValue < min || paramValue > max) {
            throw exceptionCreator?.Invoke() ?? new ArgumentOutOfRangeException(message: string.IsNullOrWhiteSpace(message)
                                                                                    ? string.Format(ARG_OUT_OF_RANGE_MESSAGE, min, max)
                                                                                    : message,
                                                                                paramName: paramName,
                                                                                actualValue: paramValue);
        }

        return paramValue;
    }

    [DebuggerStepThrough]
    public int Zero(int paramValue,
                    [CallerArgumentExpression(nameof(paramValue))] string? paramName = null,
                    string? message = null,
                    Func<Exception>? exceptionCreator = null) {
        if (paramValue == 0) {
            throw exceptionCreator?.Invoke() ?? new ArgumentException(message: string.IsNullOrWhiteSpace(message)
                                                                          ? ARG_ZERO_MESSAGE
                                                                          : message,
                                                                      paramName: paramName);
        }

        return paramValue;
    }

    [DebuggerStepThrough]
    public double Zero(double paramValue,
                       [CallerArgumentExpression(nameof(paramValue))] string? paramName = null,
                       string? message = null,
                       Func<Exception>? exceptionCreator = null) {
        if (paramValue == 0D) {
            throw exceptionCreator?.Invoke() ?? new ArgumentException(message: string.IsNullOrWhiteSpace(message)
                                                                          ? ARG_ZERO_MESSAGE
                                                                          : message,
                                                                      paramName: paramName);
        }

        return paramValue;
    }

    [DebuggerStepThrough]
    public decimal Zero(decimal paramValue,
                        [CallerArgumentExpression(nameof(paramValue))] string? paramName = null,
                        string? message = null,
                        Func<Exception>? exceptionCreator = null) {
        if (paramValue == 0M) {
            throw exceptionCreator?.Invoke() ?? new ArgumentException(message: string.IsNullOrWhiteSpace(message)
                                                                          ? ARG_ZERO_MESSAGE
                                                                          : message,
                                                                      paramName: paramName);
        }

        return paramValue;
    }

    [DebuggerStepThrough]
    public TimeSpan Zero(TimeSpan paramValue,
                         [CallerArgumentExpression(nameof(paramValue))] string? paramName = null,
                         string? message = null,
                         Func<Exception>? exceptionCreator = null) {
        if (paramValue == TimeSpan.Zero) {
            throw exceptionCreator?.Invoke() ?? new ArgumentException(message: string.IsNullOrWhiteSpace(message)
                                                                          ? ARG_ZERO_MESSAGE
                                                                          : message,
                                                                      paramName: paramName);
        }

        return paramValue;
    }

    [DebuggerStepThrough]
    public TEnum UndefinedEnum<TEnum>(int paramValue,
                                      [CallerArgumentExpression(nameof(paramValue))] string? paramName = null,
                                      Func<Exception>? exceptionCreator = null)
        where TEnum : struct, Enum {
        if (!Enum.IsDefined(typeof(TEnum), paramValue)) {
            throw exceptionCreator?.Invoke() ?? new InvalidEnumArgumentException(paramName, paramValue, typeof(TEnum));
        }

        return (TEnum)Enum.ToObject(typeof(TEnum), paramValue);
    }

    [DebuggerStepThrough]
    public TEnum UndefinedEnum<TEnum>(string paramValue,
                                      [CallerArgumentExpression(nameof(paramValue))] string? paramName = null,
                                      string? message = null,
                                      Func<Exception>? exceptionCreator = null)
        where TEnum : struct, Enum {
        if (!Enum.IsDefined(typeof(TEnum), paramValue)) {
            throw exceptionCreator?.Invoke() ?? new InvalidEnumArgumentException(message: string.IsNullOrWhiteSpace(message)
                                                                                     ? string.Format(ARG_UNDEFINED_ENUM_MESSAGE, paramName, paramValue, typeof(TEnum))
                                                                                     : message);
        }

        return (TEnum)Enum.ToObject(typeof(TEnum), paramValue);
    }
}