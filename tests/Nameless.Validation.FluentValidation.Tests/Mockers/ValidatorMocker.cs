using FluentValidation;
using FluentValidation.Results;
using Moq;
using Nameless.Testing.Tools.Mockers;

namespace Nameless.Validation.FluentValidation.Mockers;

public sealed class ValidatorMocker<T> : Mocker<IValidator<T>> {
    public ValidatorMocker() {
        MockInstance
            .Setup(mock => mock.CanValidateInstancesOfType(typeof(T)))
            .Returns(value: true);
    }

    public ValidatorMocker<T> WithSuccessfulValidateAsync() {
        MockInstance
            .Setup(mock => mock.ValidateAsync(It.IsAny<IValidationContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new global::FluentValidation.Results.ValidationResult());

        return this;
    }

    public ValidatorMocker<T> WithFailureValidateAsync() {
        MockInstance
            .Setup(mock => mock.ValidateAsync(It.IsAny<IValidationContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new global::FluentValidation.Results.ValidationResult {
                Errors = [new ValidationFailure(propertyName: "Property", errorMessage: "Error Message")]
            });

        return this;
    }

    public ValidatorMocker<T> WithFailureValidateAsync(params List<ValidationFailure> failures) {
        MockInstance
            .Setup(mock => mock.ValidateAsync(It.IsAny<IValidationContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new global::FluentValidation.Results.ValidationResult { Errors = failures });

        return this;
    }
}