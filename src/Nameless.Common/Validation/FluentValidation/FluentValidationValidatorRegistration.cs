using Nameless.Registration;

namespace Nameless.Validation.FluentValidation;

/// <summary>
///     Validation registration options.
/// </summary>
public class FluentValidationValidatorRegistration : AssemblyScanAware<FluentValidationValidatorRegistration> {
    private readonly HashSet<Type> _validators = [];

    /// <summary>
    ///     Gets the registered validator types.
    /// </summary>
    public IReadOnlyCollection<Type> Validators => UseAssemblyScan
        ? ExecuteAssemblyScan(typeof(IFluentValidationValidator))
        : _validators;

    /// <summary>
    ///     Registers a validator by generic type parameter.
    /// </summary>
    /// <typeparam name="TFluentValidationValidator">The validator type.</typeparam>
    /// <returns>
    ///     The current <see cref="FluentValidationValidatorRegistration"/> instance so other
    ///     actions can be chained.
    /// </returns>
    public FluentValidationValidatorRegistration WithFluentValidationValidator<TFluentValidationValidator>()
        where TFluentValidationValidator : IFluentValidationValidator {
        return WithFluentValidationValidator(typeof(TFluentValidationValidator));
    }

    /// <summary>
    ///     Registers a validator by type.
    /// </summary>
    /// <param name="type">The validator type.</param>
    /// <returns>
    ///     The current <see cref="FluentValidationValidatorRegistration"/> instance so other
    ///     actions can be chained.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     if <paramref name="type"/> is not assignable from <see cref="IFluentValidationValidator"/>,
    ///     is an open generic type, or is a non-concrete type.
    /// </exception>
    public FluentValidationValidatorRegistration WithFluentValidationValidator(Type type) {
        Throws.When.IsNotAssignableFrom(type, typeof(IFluentValidationValidator));
        Throws.When.IsOpenGenericType(type);
        Throws.When.IsNonConcreteType(type);

        _validators.Add(type);
        
        return this;
    }
}