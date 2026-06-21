using FluentValidation;
using Nameless.Registration;

namespace Nameless.Validation.FluentValidation;

/// <summary>
///     Validation registration options.
/// </summary>
[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public class ValidationRegistration : AssemblyScanAware<ValidationRegistration> {
    private readonly HashSet<Type> _validators = [];

    /// <summary>
    ///     Gets the registered validator types.
    /// </summary>
    public IReadOnlyCollection<Type> Validators => _validators;

    /// <summary>
    ///     Registers a validator by generic type parameter.
    /// </summary>
    /// <typeparam name="TValidator">The validator type.</typeparam>
    /// <returns>
    ///     The current <see cref="ValidationRegistration"/> instance so other
    ///     actions can be chained.
    /// </returns>
    public ValidationRegistration RegisterValidator<TValidator>()
        where TValidator : IValidator {
        return RegisterValidator(typeof(TValidator));
    }

    /// <summary>
    ///     Registers a validator by type.
    /// </summary>
    /// <param name="type">The validator type.</param>
    /// <returns>
    ///     The current <see cref="ValidationRegistration"/> instance so other
    ///     actions can be chained.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     if <paramref name="type"/> is not assignable from <see cref="IValidator"/>,
    ///     is an open generic type, or is a non-concrete type.
    /// </exception>
    public ValidationRegistration RegisterValidator(Type type) {
        Throws.When.IsNotAssignableFrom(type, typeof(IValidator));
        Throws.When.IsOpenGenericType(type);
        Throws.When.IsNonConcreteType(type);

        _validators.Add(type);
        
        return this;
    }
}