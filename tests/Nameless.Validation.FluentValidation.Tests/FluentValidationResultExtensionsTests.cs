using FluentValidation.Results;

namespace Nameless.Validation.FluentValidation;

public class FluentValidationResultExtensionsTests {
    [Fact]
    public void
        WhenConvertingToValidationResult_WhenFluentValidationResultsAreValid_ThenReturnsSuccessfulValidationResult() {
        // arrange
        var results = new[] { new global::FluentValidation.Results.ValidationResult() };

        // act
        var actual = results.ToValidationResult();

        // assert
        Assert.Multiple(() => {
            Assert.True(actual.Success);
            Assert.Empty(actual.Errors);
        });
    }

    [Fact]
    public void
        WhenConvertingToValidationResult_WhenFluentValidationResultsAreInvalid_ThenReturnsFailureValidationResult() {
        // arrange
        var results = new[] {
            new global::FluentValidation.Results.ValidationResult {
                Errors = [new ValidationFailure(propertyName: "Property", errorMessage: "Error Message")]
            }
        };

        // act
        var actual = results.ToValidationResult();

        // assert
        Assert.Multiple(() => {
            Assert.False(actual.Success);
            Assert.NotEmpty(actual.Errors);
        });
    }
}