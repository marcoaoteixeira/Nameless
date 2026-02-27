using FluentValidation;
using Nameless.Registration;

namespace Nameless.Validation.FluentValidation;

/// <summary>
///     Validation registration options.
/// </summary>
public class ValidationRegistrationSettings : AssemblyScanAware<ValidationRegistrationSettings> {
    private readonly HashSet<Type> _validators = [];

    public IReadOnlyCollection<Type> Validators => UseAssemblyScan
        ? GetImplementationsFor<IValidator>(includeGenericDefinition: false)
        : _validators;

    public ValidationRegistrationSettings RegisterValidator<TValidator>()
        where TValidator : IValidator {
        return RegisterValidator(typeof(TValidator));
    }

    public ValidationRegistrationSettings RegisterValidator(Type type) {
        Throws.When.IsNotAssignableFrom(type, typeof(IValidator));
        Throws.When.IsOpenGenericType(type);
        Throws.When.IsNonConcreteType(type);

        _validators.Add(type);
        
        return this;
    }
}