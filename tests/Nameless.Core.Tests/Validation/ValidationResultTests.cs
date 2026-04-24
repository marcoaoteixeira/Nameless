using Nameless.ObjectModel;

namespace Nameless.Validation;

public class ValidationResultTests {
    // ─── Successful ──────────────────────────────────────────────────────────

    [Fact]
    public void Successful_HasSuccessTrue() {
        // act
        var result = ValidationResult.Successful;

        // assert
        Assert.True(result.Success);
    }

    [Fact]
    public void Successful_ValueIsNothingValue() {
        // act
        var result = ValidationResult.Successful;

        // assert
        Assert.Equal(Nothing.Value, result.Value);
    }

    // ─── Implicit from Error ─────────────────────────────────────────────────

    [Fact]
    public void ImplicitConversion_FromSingleError_HasSuccessFalse() {
        // act
        ValidationResult result = Error.Validation("something is wrong");

        // assert
        Assert.False(result.Success);
    }

    [Fact]
    public void ImplicitConversion_FromSingleError_ErrorIsPresent() {
        // arrange
        var error = Error.Validation("something is wrong");

        // act
        ValidationResult result = error;

        // assert
        Assert.Single(result.Errors);
    }

    // ─── Implicit from Error[] ───────────────────────────────────────────────

    [Fact]
    public void ImplicitConversion_FromErrorArray_HasSuccessFalse() {
        // arrange
        var errors = new[] {
            Error.Validation("error one"),
            Error.Missing("error two")
        };

        // act
        ValidationResult result = errors;

        // assert
        Assert.False(result.Success);
    }

    [Fact]
    public void ImplicitConversion_FromErrorArray_AllErrorsPresent() {
        // arrange
        var errors = new[] {
            Error.Validation("error one"),
            Error.Missing("error two")
        };

        // act
        ValidationResult result = errors;

        // assert
        Assert.Equal(2, result.Errors.Length);
    }
}
