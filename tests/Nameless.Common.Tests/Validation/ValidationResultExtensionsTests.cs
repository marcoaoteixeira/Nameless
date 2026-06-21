using Nameless.ObjectModel;

namespace Nameless.Validation;

public class ValidationResultExtensionsTests {
    // --- ToDictionary ---

    [Fact]
    public void ToDictionary_WhenSuccessful_ReturnsEmptyDictionary() {
        // arrange
        var result = ValidationResult.Successful;

        // act
        var dict = result.ToDictionary();

        // assert
        Assert.Empty(dict);
    }

    [Fact]
    public void ToDictionary_WhenFailed_GroupsErrorsByCode() {
        // arrange
        ValidationResult result = new[] {
            Error.Validation("Name is required", "Name"),
            Error.Validation("Name is too short", "Name"),
            Error.Validation("Email is invalid", "Email")
        };

        // act
        var dict = result.ToDictionary();

        // assert
        Assert.Multiple(() => {
            Assert.True(dict.ContainsKey("Name"));
            Assert.True(dict.ContainsKey("Email"));
            Assert.Equal(2, dict["Name"].Length);
            Assert.Single(dict["Email"]);
        });
    }

    [Fact]
    public void ToDictionary_WhenErrorsHaveNullCode_UsesEmptyStringAsKey() {
        // arrange
        ValidationResult result = new[] {
            Error.Validation("some error")
        };

        // act
        var dict = result.ToDictionary();

        // assert
        Assert.True(dict.ContainsKey(string.Empty));
    }

    // --- Aggregate ---

    [Fact]
    public void Aggregate_WithAllSuccessful_ReturnsSuccessfulResult() {
        // arrange
        var results = new[] {
            ValidationResult.Successful,
            ValidationResult.Successful
        };

        // act
        var aggregated = results.Aggregate();

        // assert
        Assert.True(aggregated.Success);
    }

    [Fact]
    public void Aggregate_WithSomeFailed_ReturnsCombinedErrors() {
        // arrange
        var results = new ValidationResult[] {
            Error.Validation("error one"),
            ValidationResult.Successful,
            Error.Missing("error two")
        };

        // act
        var aggregated = results.Aggregate();

        // assert
        Assert.Multiple(() => {
            Assert.False(aggregated.Success);
            Assert.Equal(2, aggregated.Errors.Length);
        });
    }

    [Fact]
    public void Aggregate_WithEmptyCollection_ReturnsSuccessful() {
        // arrange
        var results = Array.Empty<ValidationResult>();

        // act
        var aggregated = results.Aggregate();

        // assert
        Assert.True(aggregated.Success);
    }
}
