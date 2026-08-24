using Nameless.Registration;

namespace Nameless.Validation.FluentValidation;

/// <summary>
///     Validation registration options.
/// </summary>
public class ValidatorRegistration : AssemblyScanAware<ValidatorRegistration> {
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
    ///     The current <see cref="ValidatorRegistration"/> instance so other
    ///     actions can be chained.
    /// </returns>
    public ValidatorRegistration RegisterValidator<TValidator>()
        where TValidator : IFluentValidator {
        return RegisterValidator(typeof(TValidator));
    }

    /// <summary>
    ///     Registers a validator by type.
    /// </summary>
    /// <param name="type">The validator type.</param>
    /// <returns>
    ///     The current <see cref="ValidatorRegistration"/> instance so other
    ///     actions can be chained.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     if <paramref name="type"/> is not assignable from <see cref="IFluentValidator"/>,
    ///     is an open generic type, or is a non-concrete type.
    /// </exception>
    public ValidatorRegistration RegisterValidator(Type type) {
        Throws.When.IsNotAssignableFrom(type, typeof(IFluentValidator));
        Throws.When.IsOpenGenericType(type);
        Throws.When.IsNonConcreteType(type);

        _validators.Add(type);
        
        return this;
    }
}