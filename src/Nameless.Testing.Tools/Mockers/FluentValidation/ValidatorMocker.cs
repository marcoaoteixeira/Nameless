using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace Nameless.Testing.Tools.Mockers.FluentValidation;

public class ValidatorMocker<TType> : Mocker<IValidator<TType>> {
    public ValidatorMocker<TType> WithCanValidateInstancesOfType(bool returnValue) {
        MockInstance
            .Setup(mock => mock.CanValidateInstancesOfType(It.IsAny<Type>()))
            .Returns(returnValue);

        return this;
    }

    public ValidatorMocker<TType> WithCanValidateInstancesOfType(Type type, bool returnValue) {
        MockInstance
            .Setup(mock => mock.CanValidateInstancesOfType(type))
            .Returns(returnValue);

        return this;
    }

    public ValidatorMocker<TType> WithValidateAsync(ValidationFailure[] failures) {
        MockInstance
            .Setup(mock => mock.ValidateAsync(It.IsAny<IValidationContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(failures));

        return this;
    }

    public ValidatorMocker<TType> WithValidateAsync() {
        return WithValidateAsync([]);
    }
}
