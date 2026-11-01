using Nameless.Contracts;
using Nameless.Registration;

namespace Nameless.Validation.FluentValidation;

/// <summary>
///     Validation registration options.
/// </summary>
public class FluentValidationFeatureOptions : AssemblyScanAware<FluentValidationFeatureOptions> {
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
    ///     The current <see cref="FluentValidationFeatureOptions"/> instance so other
    ///     actions can be chained.
    /// </returns>
    public FluentValidationFeatureOptions WithValidator<TFluentValidationValidator>()
        where TFluentValidationValidator : IFluentValidationValidator {
        return WithValidator(typeof(TFluentValidationValidator));
    }

    /// <summary>
    ///     Registers a validator by type.
    /// </summary>
    /// <param name="type">The validator type.</param>
    /// <returns>
    ///     The current <see cref="FluentValidationFeatureOptions"/> instance so other
    ///     actions can be chained.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     if <paramref name="type"/> is not assignable from <see cref="IFluentValidationValidator"/>,
    ///     is an open generic type, or is a non-concrete type.
    /// </exception>
    public FluentValidationFeatureOptions WithValidator(Type type) {
        Throws.When.OpenGenericType(type);
        Throws.When.NonConcreteType(type);
        Throws.When.NonAssignableFrom(type, typeof(IFluentValidationValidator));

        _validators.Add(type);

        return this;
    }
}