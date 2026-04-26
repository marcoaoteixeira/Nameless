using FluentValidation;
using Nameless.Registration;

namespace Nameless.Validation.FluentValidation;

/// <summary>
///     Validation registration options.
/// </summary>
public class ValidationRegistration : AssemblyScanAware<ValidationRegistration> {
    private readonly HashSet<Type> _validators = [];

    public IReadOnlyCollection<Type> Validators => _validators;

    public ValidationRegistration RegisterValidator<TValidator>()
        where TValidator : IValidator {
        return RegisterValidator(typeof(TValidator));
    }

    public ValidationRegistration RegisterValidator(Type type) {
        Throws.When.IsNotAssignableFrom(type, typeof(IValidator));
        Throws.When.IsOpenGenericType(type);
        Throws.When.IsNonConcreteType(type);

        _validators.Add(type);
        
        return this;
    }
}