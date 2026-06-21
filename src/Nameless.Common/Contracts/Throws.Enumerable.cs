// ReSharper disable ClassCannotBeInstantiated
#pragma warning disable CA1822

using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Nameless;

public sealed partial class Throws {
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
    public TValue NullOrEmpty<TValue>([NotNull] TValue? paramValue, [CallerArgumentExpression(nameof(paramValue))] string? paramName = null, string? message = null, Func<Exception>? exceptionCreator = null) where TValue : class, IEnumerable {
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
}
