// ReSharper disable ClassCannotBeInstantiated
#pragma warning disable CA1822

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Nameless;

public sealed partial class Throws {
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
    public Guid NullOrEmpty([NotNull] Guid? paramValue, [CallerArgumentExpression(nameof(paramValue))] string? paramName = null, string? message = null, Func<Exception>? exceptionCreator = null) {
        Null(paramValue, paramName, message, exceptionCreator);

        if (paramValue != Guid.Empty) {
            return paramValue.Value;
        }

        throw exceptionCreator?.Invoke()
              ?? new ArgumentException(string.IsNullOrWhiteSpace(message)
                      ? PARAM_EMPTY_MESSAGE
                      : message,
                  paramName);
    }
}
