using FluentValidation.Results;
using Nameless.Testing.Tools.Attributes;
using Nameless.Validation.FluentValidation;
using FvValidationResult = FluentValidation.Results.ValidationResult;

namespace Nameless.Validation.FluentValidation;

[UnitTest]
public class FluentValidationResultExtensionsTests {
    [Fact]
    public void Aggregate_WithEmptyList_ReturnsSuccessResult() {
        // arrange
        var results = Array.Empty<ValidationResult>();

        // act
        var actual = results.Aggregate();

        // assert
        Assert.True(actual.Success);
    }

    [Fact]
    public void Aggregate_WithAllPassingResults_ReturnsSuccessResult() {
        // arrange
        var results = new[] {
            new FvValidationResult(),
            new FvValidationResult()
        };

        // act
        var actual = results.Aggregate();

        // assert
        Assert.True(actual.Success);
    }

    [Fact]
    public void Aggregate_WithOneFailingResult_ReturnsFailure() {
        // arrange
        var failure = new ValidationFailure("Name", "Name is required");
        var results = new[] {
            new FvValidationResult(),
            new FvValidationResult([failure])
        };

        // act
        var actual = results.Aggregate();

        // assert
        Assert.Multiple(() => {
            Assert.False(actual.Success);
            Assert.Single(actual.Errors);
            Assert.Equal("Name is required", actual.Errors[0].Message);
            Assert.Equal("Name", actual.Errors[0].Code);
        });
    }

    [Fact]
    public void Aggregate_WithMultipleFailingResults_MergesAllErrors() {
        // arrange
        var firstFailures = new[] {
            new ValidationFailure("Name", "Name is required"),
            new ValidationFailure("Email", "Email is invalid")
        };
        var secondFailures = new[] {
            new ValidationFailure("Age", "Age must be positive")
        };
        var results = new[] {
            new FvValidationResult(firstFailures),
            new FvValidationResult(secondFailures)
        };

        // act
        var actual = results.Aggregate();

        // assert
        Assert.Multiple(() => {
            Assert.False(actual.Success);
            Assert.Equal(3, actual.Errors.Length);
        });
    }
}
