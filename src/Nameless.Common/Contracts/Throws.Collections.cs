// ReSharper disable ClassCannotBeInstantiated
#pragma warning disable CA1822

using System.Collections;
using System.Runtime.CompilerServices;

namespace Nameless;

public sealed partial class Throws {
    /// <summary>
    ///     Throws exception if <paramref name="paramValue"/> is empty.
    /// </summary>
    /// <param name="paramValue">
    ///     The parameter value.
    /// </param>
    /// <param name="paramName">
    ///     The parameter name.
    /// </param>
    /// <param name="message">
    ///     The message.
    /// </param>
    /// <param name="exceptionCreator">
    ///     The exception factory.
    /// </param>
    /// <returns>
    ///     The current instance, if pass validation.
    /// </returns>
    public TValue Empty<TValue>(TValue paramValue, [CallerArgumentExpression(nameof(paramValue))] string? paramName = null, string? message = null, Func<Exception>? exceptionCreator = null)
        where TValue : IEnumerable {
        var enumerator = paramValue.GetEnumerator();
        var move = enumerator.MoveNext();

        if (enumerator is IDisposable disposable) {
            disposable.Dispose();
        }

        if (move) {
            return paramValue;
        }

        throw exceptionCreator?.Invoke()
              ?? new ArgumentException(string.IsNullOrWhiteSpace(message)
                      ? PARAM_ENUMERABLE_EMPTY_MESSAGE
                      : message,
                  paramName);
    }

    /// <summary>
    ///     Throws exception if <paramref name="paramValue"/> contains
    ///     any <see langword="null"/>, empty or white space value.
    /// </summary>
    /// <param name="paramValue">
    ///     The parameter value.
    /// </param>
    /// <param name="paramName">
    ///     The parameter name.
    /// </param>
    /// <param name="message">
    ///     The message.
    /// </param>
    /// <param name="exceptionCreator">
    ///     The exception factory.
    /// </param>
    /// <returns>
    ///     The current instance, if pass validation.
    /// </returns>
    public string[] AnyNullOrWhiteSpace(string[] paramValue, [CallerArgumentExpression(nameof(paramValue))] string? paramName = null, string? message = null, Func<Exception>? exceptionCreator = null) {
        if (!paramValue.Any(string.IsNullOrWhiteSpace)) {
            return paramValue;
        }

        throw exceptionCreator?.Invoke()
              ?? new ArgumentException(string.IsNullOrWhiteSpace(message)
                      ? PARAM_ANY_NULL_WHITESPACE_MESSAGE
                      : message,
                  paramName);
    }
}
