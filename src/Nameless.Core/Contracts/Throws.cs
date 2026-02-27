#pragma warning disable CA1822

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Nameless;

/// <summary>
///     A guard clause (https://en.wikipedia.org/wiki/Guard_(computer_science))
///     is a software pattern that simplifies complex functions by
///     "failing fast", checking for invalid inputs up front and immediately
///     failing if any are found.
/// </summary>
public sealed partial class Throws {
    private const string PARAM_EMPTY_MESSAGE = "Parameter cannot be empty.";
    private const string PARAM_NULL_MESSAGE = "Parameter cannot be null.";
    private const string PARAM_DEFAULT_MESSAGE = "Parameter cannot be default.";
    private const string PARAM_LOWER_OR_EQUAL_MESSAGE = "Parameter cannot be lower or equal to '{0}'.";
    private const string PARAM_GREATER_OR_EQUAL_MESSAGE = "Parameter cannot be greater or equal to '{0}'.";
    private const string PARAM_LOWER_THAN_MESSAGE = "Parameter cannot be lower than '{0}'.";
    private const string PARAM_GREATER_THAN_MESSAGE = "Parameter cannot be greater than '{0}'.";
    private const string PARAM_OUT_OF_RANGE_MESSAGE = "Parameter must be between minimum value of '{0}' and maximum value of '{1}'.";
    private const string PARAM_ZERO_MESSAGE = "Parameter cannot be zero value.";
    private const string PARAM_NEGATIVE_MESSAGE = "Parameter cannot be negative value.";
    private const string PARAM_WHITE_SPACES_MESSAGE = "Parameter cannot be white spaces.";
    private const string PARAM_NO_MATCHING_PATTERN_MESSAGE = "Parameter does not match pattern: {0}";
    private const string PARAM_IS_NON_CONCRETE_TYPE = "Type '{0}' must be a concrete type.";
    private const string PARAM_IS_NON_OPEN_GENERIC_TYPE = "Type '{0}' must be an open generic type.";
    private const string PARAM_IS_OPEN_GENERIC_TYPE = "Type '{0}' must not be an open generic type.";
    private const string PARAM_IS_NOT_ASSIGNABLE_TYPE = "Type '{0}' must be assignable to '{1}'.";
    private const string PARAM_HAS_NO_PARAMETERLESS_CONSTRUCTOR = "Type '{0}' must have a parameteless constructor.";

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
    public TValue Null<TValue>([NotNull] TValue? paramValue, [CallerArgumentExpression(nameof(paramValue))] string? paramName = null, string? message = null, Func<Exception>? exceptionCreator = null) where TValue : class {
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
    public TValue Null<TValue>([NotNull] TValue? paramValue, [CallerArgumentExpression(nameof(paramValue))] string? paramName = null, string? message = null, Func<Exception>? exceptionCreator = null) where TValue : struct {
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
    public TValue Default<TValue>([NotNull] TValue? paramValue, [CallerArgumentExpression(nameof(paramValue))] string? paramName = null, string? message = null, Func<Exception>? exceptionCreator = null) {
        if (!EqualityComparer<TValue?>.Default.Equals(paramValue, y: default) && paramValue is not null) {
            return paramValue;
        }

        throw exceptionCreator?.Invoke()
              ?? new ArgumentException(string.IsNullOrWhiteSpace(message)
                      ? PARAM_DEFAULT_MESSAGE
                      : message,
                  paramName);
    }
}