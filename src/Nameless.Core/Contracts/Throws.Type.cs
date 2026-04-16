// ReSharper disable ClassCannotBeInstantiated
#pragma warning disable CA1822

using System.Runtime.CompilerServices;

namespace Nameless;

public sealed partial class Throws {
    public Type IsNonConcreteType(Type paramValue, [CallerArgumentExpression(nameof(paramValue))] string? paramName = null, string? message = null, Func<Exception>? exceptionCreator = null) {
        Null(paramValue, paramName, message, exceptionCreator);

        if (paramValue.IsConcrete) {
            return paramValue;
        }

        throw exceptionCreator?.Invoke()
              ?? new ArgumentException(string.IsNullOrWhiteSpace(message)
                      ? string.Format(PARAM_IS_NON_CONCRETE_TYPE, paramValue.GetPrettyName())
                      : message,
                  paramName);
    }

    public Type IsNonOpenGenericType(Type paramValue, [CallerArgumentExpression(nameof(paramValue))] string? paramName = null, string? message = null, Func<Exception>? exceptionCreator = null) {
        Null(paramValue, paramName, message, exceptionCreator);

        if (paramValue.IsOpenGeneric) {
            return paramValue;
        }

        throw exceptionCreator?.Invoke()
              ?? new ArgumentException(string.IsNullOrWhiteSpace(message)
                      ? string.Format(PARAM_IS_NON_OPEN_GENERIC_TYPE, paramValue.GetPrettyName())
                      : message,
                  paramName);
    }

    public Type IsOpenGenericType(Type paramValue, [CallerArgumentExpression(nameof(paramValue))] string? paramName = null, string? message = null, Func<Exception>? exceptionCreator = null) {
        Null(paramValue, paramName, message, exceptionCreator);

        if (!paramValue.IsOpenGeneric) {
            return paramValue;
        }

        throw exceptionCreator?.Invoke()
              ?? new ArgumentException(string.IsNullOrWhiteSpace(message)
                      ? string.Format(PARAM_IS_OPEN_GENERIC_TYPE, paramValue.GetPrettyName())
                      : message,
                  paramName);
    }

    public Type IsNotAssignableFrom(Type paramValue, Type lhs, [CallerArgumentExpression(nameof(paramValue))] string? paramName = null, string? message = null, Func<Exception>? exceptionCreator = null) {
        Null(paramValue, paramName, message, exceptionCreator);

        if (lhs.IsAssignableFrom(paramValue)) {
            return paramValue;
        }

        throw exceptionCreator?.Invoke()
              ?? new ArgumentException(string.IsNullOrWhiteSpace(message)
                      ? string.Format(PARAM_IS_NOT_ASSIGNABLE_TYPE, paramValue.GetPrettyName(), lhs.GetPrettyName())
                      : message,
                  paramName);
    }

    public Type IsNotAssignableFromGeneric(Type paramValue, Type lhs, [CallerArgumentExpression(nameof(paramValue))] string? paramName = null, string? message = null, Func<Exception>? exceptionCreator = null) {
        Null(paramValue, paramName, message, exceptionCreator);

        if (lhs.IsAssignableFromGeneric(paramValue)) {
            return paramValue;
        }

        throw exceptionCreator?.Invoke()
              ?? new ArgumentException(string.IsNullOrWhiteSpace(message)
                      ? string.Format(PARAM_IS_NOT_ASSIGNABLE_TYPE, paramValue.GetPrettyName(), lhs.GetPrettyName())
                      : message,
                  paramName);
    }

    public Type HasNoParameterlessConstructor(Type paramValue, [CallerArgumentExpression(nameof(paramValue))] string? paramName = null, string? message = null, Func<Exception>? exceptionCreator = null) {
        Null(paramValue, paramName, message, exceptionCreator);

        if (paramValue.HasParameterlessConstructor()) {
            return paramValue;
        }

        throw exceptionCreator?.Invoke()
              ?? new ArgumentException(string.IsNullOrWhiteSpace(message)
                      ? string.Format(PARAM_HAS_NO_PARAMETERLESS_CONSTRUCTOR, paramValue.GetPrettyName())
                      : message,
                  paramName);
    }
}
