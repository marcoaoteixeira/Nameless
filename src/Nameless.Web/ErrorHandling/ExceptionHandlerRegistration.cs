using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Nameless.Registration;

namespace Nameless.Web.ErrorHandling;

public class ExceptionHandlerRegistration : AssemblyScanAware<ExceptionHandlerRegistration> {
    private readonly List<Type> _exceptionHandlers = [];

    public IReadOnlyCollection<Type> ExceptionHandlers => _exceptionHandlers;

    /// <summary>
    ///     Gets or sets a delegate to configure the exception handler options.
    /// </summary>
    public Action<ExceptionHandlerOptions>? ConfigureExceptionHandlerOptions { get; set; }

    /// <summary>
    ///     Registers an exception handler.
    /// </summary>
    /// <typeparam name="TExceptionHandler">
    ///     The type of the exception handler to register.
    /// </typeparam>
    /// <returns>
    ///     The current <see cref="ExceptionHandlerRegistration"/> so other
    ///     actions can be chained.
    /// </returns>
    public ExceptionHandlerRegistration RegisterExceptionHandler<TExceptionHandler>()
        where TExceptionHandler : IExceptionHandler {
        return RegisterExceptionHandler(typeof(TExceptionHandler));
    }

    /// <summary>
    ///     Registers an exception handler.
    /// </summary>
    /// <param name="type">
    ///     The concrete exception handler type to register.
    /// </param>
    /// <returns>
    ///     The current <see cref="ExceptionHandlerRegistration"/> so other
    ///     actions can be chained.
    /// </returns>
    /// <exception cref="ArgumentException">
    ///     if <paramref name="type"/> is not assignable from
    ///     <see cref="IExceptionHandler"/>, is an open generic type, or
    ///     is a non-concrete type.
    /// </exception>
    public ExceptionHandlerRegistration RegisterExceptionHandler(Type type) {
        Throws.When.IsNotAssignableFrom(type, typeof(IExceptionHandler));
        Throws.When.IsOpenGenericType(type);
        Throws.When.IsNonConcreteType(type);

        if (!_exceptionHandlers.Contains(type)) {
            _exceptionHandlers.Add(type);
        }

        return this;
    }
}
