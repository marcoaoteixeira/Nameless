using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Nameless.Contracts;

/// <summary>
///     A guard clause (https://en.wikipedia.org/wiki/Guard_(computer_science))
///     is a software pattern that simplifies complex functions by
///     "failing fast", checking for invalid inputs up front and immediately
///     failing if any are found.
/// </summary>
public sealed partial class Throws {
    internal static class Messages {
        public static string ParameterCannotBeNull() {
            return "Parameter cannot be null.";
        }

        public static string ParameterCannotBeLowerOrEqual(object value) {
            return $"Parameter cannot be lower or equal to {value}.";
        }

        public static string ParameterCannotBeGreaterOrEqual(object value) {
            return $"Parameter cannot be greater or equal to {value}.";
        }

        public static string ParameterCannotBeLowerThan(object value) {
            return $"Parameter cannot be lower than {value}.";
        }

        public static string ParameterCannotBeGreaterThan(object value) {
            return $"Parameter cannot be greater than {value}.";
        }

        public static string ParameterOutOfRange(object min, object max, bool excludeBounds) {
            return $"Parameter must be between minimum value of {min} and maximum value of {max}. (Exclude bounds? {excludeBounds})";
        }

        public static string ParameterCannotBeZero() {
            return "Parameter cannot be zero value.";
        }

        public static string ParameterCannotBeNegative() {
            return "Parameter cannot be negative value.";
        }

        public static string ParameterMustBeConcreteType(Type value) {
            return $"Parameter ({value.GetPrettyName()}) must be a concrete type.";
        }

        public static string ParameterMustBeOpenGenericType(Type value) {
            return $"Parameter ({value.GetPrettyName()}) must be an open generic type.";
        }

        public static string ParameterMustBeClosedType(Type value) {
            return $"Parameter ({value.GetPrettyName()}) must be a closed type.";
        }

        public static string ParameterMustBeAssignableFrom(Type value, Type lhs) {
            return $"Parameter ({value.GetPrettyName()}) must be assignable to {lhs.GetPrettyName()}.";
        }

        public static string ParameterMustHaveParameterlessConstructor(Type value) {
            return $"Parameter ({value.GetPrettyName()}) must have parameterless constructor.";
        }
    }

    /// <summary>
    ///     Gets the unique instance of <see cref="Throws" />.
    /// </summary>
    public static Throws When { get; } = new();

    // Explicit static constructor to tell the C# compiler
    // not to mark type as beforefieldinit
    static Throws() { }

    private Throws() { }

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
    public TValue Null<TValue>([NotNull] TValue? paramValue, [CallerArgumentExpression(nameof(paramValue))] string? paramName = null, string? message = null, Func<Exception>? exceptionCreator = null) {
        if (paramValue is not null) {
            return paramValue;
        }

        throw exceptionCreator?.Invoke()
              ?? new ArgumentNullException(paramName, string.IsNullOrWhiteSpace(message)
                  ? Messages.ParameterCannotBeNull()
                  : message);
    }
}