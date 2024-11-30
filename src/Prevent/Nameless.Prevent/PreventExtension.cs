using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Nameless;

public static class PreventExtension {
    /// <summary>
    /// Throws an <see cref="ArgumentNullException"/> if parameter <paramref name="paramValue"/> is <c>null</c>.
    /// </summary>
    /// <typeparam name="TValue">Type of the parameter value.</typeparam>
    /// <param name="_">The <see cref="Prevent"/> current instance. (ignored)</param>
    /// <param name="paramValue">The parameter value.</param>
    /// <param name="paramName">The parameter name (optional).</param>
    /// <param name="message">The exception message (optional).</param>
    /// <param name="exceptionCreator">The exception creator (optional).</param>
    /// <returns>
    /// The current <paramref name="paramValue"/>.
    /// </returns>
    [DebuggerStepThrough]
    public static TValue Null<TValue>(this Prevent _,
                                      [NotNull] TValue? paramValue,
                                      [CallerArgumentExpression(nameof(paramValue))] string? paramName = null,
                                      string? message = null,
                                      Func<Exception>? exceptionCreator = null) where TValue : class {
        if (paramValue is not null) {
            return paramValue;
        }

        throw exceptionCreator?.Invoke() ?? new ArgumentNullException(paramName: paramName,
                                                                      message: string.IsNullOrWhiteSpace(message)
                                                                          ? Constants.ArgNullMessage
                                                                          : message);
    }

    /// <summary>
    /// Throws an <see cref="ArgumentNullException"/> if parameter <paramref name="paramValue"/> is <c>null</c>.
    /// </summary>
    /// <typeparam name="TValue">Type of the parameter value.</typeparam>
    /// <param name="_">The <see cref="Prevent"/> current instance. (ignored)</param>
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
    public static TValue Null<TValue>(this Prevent _,
                                      [NotNull] TValue? paramValue,
                                      [CallerArgumentExpression(nameof(paramValue))] string? paramName = null,
                                      string? message = null,
                                      Func<Exception>? exceptionCreator = null) where TValue : struct {
        if (paramValue is not null) {
            return paramValue.Value;
        }

        throw exceptionCreator?.Invoke() ?? new ArgumentNullException(paramName: paramName,
                                                                      message: string.IsNullOrWhiteSpace(message)
                                                                          ? Constants.ArgNullMessage
                                                                          : message);
    }

    [DebuggerStepThrough]
    public static string NullOrEmpty(this Prevent _,
                                     [NotNull] string? paramValue,
                                     [CallerArgumentExpression(nameof(paramValue))] string? paramName = null,
                                     string? message = null,
                                     Func<Exception>? exceptionCreator = null) {
        Null(_, paramValue, paramName, message, exceptionCreator);

        if (paramValue.Length == 0) {
            throw exceptionCreator?.Invoke() ?? new ArgumentException(message: string.IsNullOrWhiteSpace(message)
                                                                          ? Constants.ArgEmptyMessage
                                                                          : message,
                                                                      paramName: paramName);
        }

        return paramValue;
    }

    [DebuggerStepThrough]
    public static string NullOrWhiteSpace(this Prevent _,
                                          [NotNull] string? paramValue,
                                          [CallerArgumentExpression(nameof(paramValue))] string? paramName = null,
                                          string? message = null,
                                          Func<Exception>? exceptionCreator = null) {
        NullOrEmpty(_, paramValue, paramName, message, exceptionCreator);

        if (paramValue.Trim().Length == 0) {
            throw exceptionCreator?.Invoke() ?? new ArgumentException(message: string.IsNullOrWhiteSpace(message)
                                                                          ? Constants.ArgWhiteSpacesMessage
                                                                          : message,
                                                                      paramName: paramName);
        }

        return paramValue;
    }

    [DebuggerStepThrough]
    public static Guid NullOrEmpty(this Prevent _,
                                   [NotNull] Guid? paramValue,
                                   [CallerArgumentExpression(nameof(paramValue))] string? paramName = null,
                                   string? message = null,
                                   Func<Exception>? exceptionCreator = null) {
        Null(_, paramValue, paramName, message, exceptionCreator);

        if (paramValue == Guid.Empty) {
            throw exceptionCreator?.Invoke() ?? new ArgumentException(message: string.IsNullOrWhiteSpace(message)
                                                                          ? Constants.ArgEmptyMessage
                                                                          : message,
                                                                      paramName: paramName);
        }

        return paramValue.Value;
    }

    [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
    [DebuggerStepThrough]
    public static T NullOrEmpty<T>(this Prevent _,
                                   [NotNull] T? paramValue,
                                   [CallerArgumentExpression(nameof(paramValue))] string? paramName = null,
                                   string? message = null,
                                   Func<Exception>? exceptionCreator = null) where T : class, IEnumerable {

        Null(_, paramValue, paramName, message, exceptionCreator);
        
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
                                                                          ? Constants.ArgEmptyMessage
                                                                          : message,
                                                                      paramName: paramName);
        }
    }

    [DebuggerStepThrough]
    public static T Default<T>(this Prevent _,
                               [NotNull] T? paramValue,
                               [CallerArgumentExpression(nameof(paramValue))] string? paramName = null,
                               string? message = null,
                               Func<Exception>? exceptionCreator = null) {
        if (EqualityComparer<T?>.Default.Equals(paramValue, default) || paramValue is null) {
            throw exceptionCreator?.Invoke() ?? new ArgumentException(message: string.IsNullOrWhiteSpace(message)
                                                                          ? Constants.ArgDefaultMessage
                                                                          : message,
                                                                      paramName: paramName);
        }

        return paramValue;
    }

    [DebuggerStepThrough]
    public static string NoMatchingPattern(this Prevent _,
                                           string paramValue,
                                           string pattern,
                                           [CallerArgumentExpression(nameof(paramValue))] string? paramName = null,
                                           string? message = null,
                                           Func<Exception>? exceptionCreator = null) {
        var match = Regex.Match(paramValue, pattern);
        if (!match.Success || match.Value != paramValue) {
            throw exceptionCreator?.Invoke() ?? new ArgumentException(message: string.IsNullOrWhiteSpace(message)
                                                                          ? string.Format(Constants.ArgNoMatchingPatternMessage, pattern)
                                                                          : message,
                                                                      paramName: paramName);
        }

        return paramValue;
    }

    [DebuggerStepThrough]
    public static ReadOnlySpan<char> Empty(this Prevent _,
                                           ReadOnlySpan<char> paramValue,
                                           [CallerArgumentExpression(nameof(paramValue))] string? paramName = null,
                                           string? message = null,
                                           Func<Exception>? exceptionCreator = null) {
        if (paramValue.Length == 0 || paramValue == string.Empty) {
            throw exceptionCreator?.Invoke() ?? new ArgumentException(message: string.IsNullOrWhiteSpace(message)
                                                                          ? Constants.ArgEmptyMessage
                                                                          : message,
                                                                      paramName: paramName);
        }

        return paramValue;
    }

    [DebuggerStepThrough]
    public static ReadOnlySpan<char> WhiteSpace(this Prevent _,
                                                ReadOnlySpan<char> paramValue,
                                                [CallerArgumentExpression(nameof(paramValue))] string? paramName = null,
                                                string? message = null,
                                                Func<Exception>? exceptionCreator = null) {
        if (paramValue.IsWhiteSpace()) {
            throw exceptionCreator?.Invoke() ?? new ArgumentException(message: string.IsNullOrWhiteSpace(message)
                                                                          ? Constants.ArgWhiteSpacesMessage
                                                                          : message,
                                                                      paramName: paramName);
        }

        return paramValue;
    }

    [DebuggerStepThrough]
    public static TimeSpan LowerOrEqual(this Prevent _,
                                        TimeSpan paramValue,
                                        TimeSpan to,
                                        [CallerArgumentExpression(nameof(paramValue))] string? paramName = null,
                                        string? message = null,
                                        Func<Exception>? exceptionCreator = null) {
        if (paramValue <= to) {
            throw exceptionCreator?.Invoke() ?? new ArgumentOutOfRangeException(message: string.IsNullOrWhiteSpace(message)
                                                                                    ? string.Format(Constants.ArgLowerOrEqualMessage, to)
                                                                                    : message,
                                                                                paramName: paramName,
                                                                                actualValue: paramValue);
        }

        return paramValue;
    }

    [DebuggerStepThrough]
    public static TimeSpan GreaterOrEqual(this Prevent _,
                                          TimeSpan paramValue,
                                          TimeSpan to,
                                          [CallerArgumentExpression(nameof(paramValue))] string? paramName = null,
                                          string? message = null,
                                          Func<Exception>? exceptionCreator = null) {
        if (paramValue >= to) {
            throw exceptionCreator?.Invoke() ?? new ArgumentOutOfRangeException(message: string.IsNullOrWhiteSpace(message)
                                                                                    ? string.Format(Constants.ArgGreaterOrEqualMessage, to)
                                                                                    : message,
                                                                                paramName: paramName,
                                                                                actualValue: paramValue);
        }

        return paramValue;
    }

    [DebuggerStepThrough]
    public static int LowerOrEqual(this Prevent _,
                                   int paramValue,
                                   int to,
                                   [CallerArgumentExpression(nameof(paramValue))] string? paramName = null,
                                   string? message = null,
                                   Func<Exception>? exceptionCreator = null) {
        if (paramValue <= to) {
            throw exceptionCreator?.Invoke() ?? new ArgumentOutOfRangeException(message: string.IsNullOrWhiteSpace(message)
                                                                                    ? string.Format(Constants.ArgLowerOrEqualMessage, to)
                                                                                    : message,
                                                                                paramName: paramName,
                                                                                actualValue: paramValue);
        }

        return paramValue;
    }

    [DebuggerStepThrough]
    public static int GreaterOrEqual(this Prevent _,
                                     int paramValue,
                                     int to,
                                     [CallerArgumentExpression(nameof(paramValue))] string? paramName = null,
                                     string? message = null,
                                     Func<Exception>? exceptionCreator = null) {
        if (paramValue >= to) {
            throw exceptionCreator?.Invoke() ?? new ArgumentOutOfRangeException(message: string.IsNullOrWhiteSpace(message)
                                                                                    ? string.Format(Constants.ArgGreaterOrEqualMessage, to)
                                                                                    : message,
                                                                                paramName: paramName,
                                                                                actualValue: paramValue);
        }

        return paramValue;
    }

    [DebuggerStepThrough]
    public static int LowerThan(this Prevent _,
                                int paramValue,
                                int minValue,
                                [CallerArgumentExpression(nameof(paramValue))] string? paramName = null,
                                string? message = null,
                                Func<Exception>? exceptionCreator = null) {
        if (paramValue < minValue) {
            throw exceptionCreator?.Invoke() ?? new ArgumentOutOfRangeException(message: string.IsNullOrWhiteSpace(message)
                                                                                    ? string.Format(Constants.ArgLowerThanMessage, minValue)
                                                                                    : message,
                                                                                paramName: paramName,
                                                                                actualValue: paramValue);
        }

        return paramValue;
    }

    [DebuggerStepThrough]
    public static int GreaterThan(this Prevent _,
                                  int paramValue,
                                  int maxValue,
                                  [CallerArgumentExpression(nameof(paramValue))] string? paramName = null,
                                  string? message = null,
                                  Func<Exception>? exceptionCreator = null) {
        if (paramValue > maxValue) {
            throw exceptionCreator?.Invoke() ?? new ArgumentOutOfRangeException(message: string.IsNullOrWhiteSpace(message)
                                                                                    ? string.Format(Constants.ArgGreaterThanMessage, maxValue)
                                                                                    : message,
                                                                                paramName: paramName,
                                                                                actualValue: paramValue);
        }

        return paramValue;
    }

    [DebuggerStepThrough]
    public static int OutOfRange(this Prevent _,
                                 int paramValue,
                                 int min,
                                 int max,
                                 [CallerArgumentExpression(nameof(paramValue))] string? paramName = null,
                                 string? message = null,
                                 Func<Exception>? exceptionCreator = null) {
        if (paramValue < min || paramValue > max) {
            throw exceptionCreator?.Invoke() ?? new ArgumentOutOfRangeException(message: string.IsNullOrWhiteSpace(message)
                                                                                    ? string.Format(Constants.ArgOutOfRangeMessage, min, max)
                                                                                    : message,
                                                                                paramName: paramName,
                                                                                actualValue: paramValue);
        }

        return paramValue;
    }

    [DebuggerStepThrough]
    public static int Zero(this Prevent _,
                           int paramValue,
                           [CallerArgumentExpression(nameof(paramValue))] string? paramName = null,
                           string? message = null,
                           Func<Exception>? exceptionCreator = null) {
        if (paramValue == 0) {
            throw exceptionCreator?.Invoke() ?? new ArgumentException(message: string.IsNullOrWhiteSpace(message)
                                                                          ? Constants.ArgZeroMessage
                                                                          : message,
                                                                      paramName: paramName);
        }

        return paramValue;
    }

    [DebuggerStepThrough]
    public static double Zero(this Prevent _,
                              double paramValue,
                              [CallerArgumentExpression(nameof(paramValue))] string? paramName = null,
                              string? message = null,
                              Func<Exception>? exceptionCreator = null) {
        if (paramValue == 0D) {
            throw exceptionCreator?.Invoke() ?? new ArgumentException(message: string.IsNullOrWhiteSpace(message)
                                                                          ? Constants.ArgZeroMessage
                                                                          : message,
                                                                      paramName: paramName);
        }

        return paramValue;
    }

    [DebuggerStepThrough]
    public static decimal Zero(this Prevent _,
                               decimal paramValue,
                               [CallerArgumentExpression(nameof(paramValue))] string? paramName = null,
                               string? message = null,
                               Func<Exception>? exceptionCreator = null) {
        if (paramValue == 0M) {
            throw exceptionCreator?.Invoke() ?? new ArgumentException(message: string.IsNullOrWhiteSpace(message)
                                                                          ? Constants.ArgZeroMessage
                                                                          : message,
                                                                      paramName: paramName);
        }

        return paramValue;
    }

    [DebuggerStepThrough]
    public static TimeSpan Zero(this Prevent _,
                                TimeSpan paramValue,
                                [CallerArgumentExpression(nameof(paramValue))] string? paramName = null,
                                string? message = null,
                                Func<Exception>? exceptionCreator = null) {
        if (paramValue == TimeSpan.Zero) {
            throw exceptionCreator?.Invoke() ?? new ArgumentException(message: string.IsNullOrWhiteSpace(message)
                                                                          ? Constants.ArgZeroMessage
                                                                          : message,
                                                                      paramName: paramName);
        }

        return paramValue;
    }

    [DebuggerStepThrough]
    public static TEnum UndefinedEnum<TEnum>(this Prevent _,
                                            int paramValue,
                                            [CallerArgumentExpression(nameof(paramValue))] string? paramName = null,
                                            Func<Exception>? exceptionCreator = null)
    where TEnum : struct, Enum {
        if (!Enum.IsDefined(typeof(TEnum), paramValue)) {
            throw exceptionCreator?.Invoke() ?? new InvalidEnumArgumentException(paramName, paramValue, typeof(TEnum));
        }

        return (TEnum)Enum.ToObject(typeof(TEnum), paramValue);
    }

    [DebuggerStepThrough]
    public static TEnum UndefinedEnum<TEnum>(this Prevent _,
                                             string paramValue,
                                             [CallerArgumentExpression(nameof(paramValue))] string? paramName = null,
                                             string? message = null,
                                             Func<Exception>? exceptionCreator = null)
        where TEnum : struct, Enum {
        if (!Enum.IsDefined(typeof(TEnum), paramValue)) {
            throw exceptionCreator?.Invoke() ?? new InvalidEnumArgumentException(message: string.IsNullOrWhiteSpace(message)
                                                                                     ? string.Format(Constants.ArgUndefinedEnumMessage, paramName, paramValue, typeof(TEnum))
                                                                                     : message);
        }

        return (TEnum)Enum.ToObject(typeof(TEnum), paramValue);
    }
}