using System.Runtime.CompilerServices;

namespace Nameless.Contracts;

public sealed partial class Throws {
    /// <summary>
    ///     Throws exception if <paramref name="paramValue"/> is non-concrete type.
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
    ///     The current type if pass validation.
    /// </returns>
    public Type NonConcreteType(Type paramValue, [CallerArgumentExpression(nameof(paramValue))] string? paramName = null, string? message = null, Func<Exception>? exceptionCreator = null) {
        Null(paramValue, paramName, message, exceptionCreator);

        if (paramValue.IsConcrete) {
            return paramValue;
        }

        throw exceptionCreator?.Invoke()
              ?? new ArgumentException(string.IsNullOrWhiteSpace(message)
                      ? Messages.ParameterMustBeConcreteType(paramValue)
                      : message,
                  paramName);
    }

    /// <summary>
    ///     Throws exception if <paramref name="paramValue"/> is non-open generic type.
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
    ///     The current type if pass validation.
    /// </returns>
    public Type NonOpenGenericType(Type paramValue, [CallerArgumentExpression(nameof(paramValue))] string? paramName = null, string? message = null, Func<Exception>? exceptionCreator = null) {
        Null(paramValue, paramName, message, exceptionCreator);

        if (paramValue.IsOpenGeneric) {
            return paramValue;
        }

        throw exceptionCreator?.Invoke()
              ?? new ArgumentException(string.IsNullOrWhiteSpace(message)
                      ? Messages.ParameterMustBeOpenGenericType(paramValue)
                      : message,
                  paramName);
    }

    /// <summary>
    ///     Throws exception if <paramref name="paramValue"/> is open generic type.
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
    ///     The current type if pass validation.
    /// </returns>
    public Type OpenGenericType(Type paramValue, [CallerArgumentExpression(nameof(paramValue))] string? paramName = null, string? message = null, Func<Exception>? exceptionCreator = null) {
        Null(paramValue, paramName, message, exceptionCreator);

        if (!paramValue.IsOpenGeneric) {
            return paramValue;
        }

        throw exceptionCreator?.Invoke()
              ?? new ArgumentException(string.IsNullOrWhiteSpace(message)
                      ? Messages.ParameterMustBeClosedType(paramValue)
                      : message,
                  paramName);
    }

    /// <summary>
    ///     Throws exception if <paramref name="paramValue"/> is not assignable
    ///     from <paramref name="lhs"/>.
    /// </summary>
    /// <param name="paramValue">
    ///     The parameter value.
    /// </param>
    /// <param name="lhs">
    ///     The left hand side type.
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
    ///     The current type if pass validation.
    /// </returns>
    public Type NonAssignableFrom(Type paramValue, Type lhs, [CallerArgumentExpression(nameof(paramValue))] string? paramName = null, string? message = null, Func<Exception>? exceptionCreator = null) {
        Null(paramValue, paramName, message, exceptionCreator);

        if (lhs.IsAssignableFrom(paramValue)) {
            return paramValue;
        }

        throw exceptionCreator?.Invoke()
              ?? new ArgumentException(string.IsNullOrWhiteSpace(message)
                      ? Messages.ParameterMustBeAssignableFrom(paramValue, lhs)
                      : message,
                  paramName);
    }

    /// <summary>
    ///     Throws exception if <paramref name="paramValue"/> is not assignable
    ///     from generic <paramref name="lhs"/>.
    /// </summary>
    /// <param name="paramValue">
    ///     The parameter value.
    /// </param>
    /// <param name="lhs">
    ///     The left hand side type.
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
    ///     The current type if pass validation.
    /// </returns>
    public Type NonAssignableFromGeneric(Type paramValue, Type lhs, [CallerArgumentExpression(nameof(paramValue))] string? paramName = null, string? message = null, Func<Exception>? exceptionCreator = null) {
        Null(paramValue, paramName, message, exceptionCreator);

        if (lhs.IsAssignableFromGeneric(paramValue)) {
            return paramValue;
        }

        throw exceptionCreator?.Invoke()
              ?? new ArgumentException(string.IsNullOrWhiteSpace(message)
                      ? Messages.ParameterMustBeAssignableFrom(paramValue, lhs)
                      : message,
                  paramName);
    }

    /// <summary>
    ///     Throws exception if <paramref name="paramValue"/> does not have
    ///     a parameterless constructor.
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
    ///     The current type if pass validation.
    /// </returns>
    public Type HasNoParameterlessConstructor(Type paramValue, [CallerArgumentExpression(nameof(paramValue))] string? paramName = null, string? message = null, Func<Exception>? exceptionCreator = null) {
        Null(paramValue, paramName, message, exceptionCreator);

        if (paramValue.HasParameterlessConstructor()) {
            return paramValue;
        }

        throw exceptionCreator?.Invoke()
              ?? new ArgumentException(string.IsNullOrWhiteSpace(message)
                      ? Messages.ParameterMustHaveParameterlessConstructor(paramValue)
                      : message,
                  paramName);
    }
}
