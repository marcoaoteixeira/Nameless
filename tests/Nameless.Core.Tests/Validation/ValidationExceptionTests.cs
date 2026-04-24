using Nameless.ObjectModel;

namespace Nameless.Validation;

public class ValidationExceptionTests {
    // ─── Constructor(result) ─────────────────────────────────────────────────

    [Fact]
    public void Constructor_WithResult_SetsResultProperty() {
        // arrange
        ValidationResult result = Error.Validation("invalid");

        // act
        var exception = new ValidationException(result);

        // assert
        Assert.Same(result, exception.Result);
    }

    [Fact]
    public void Constructor_WithNullResult_ThrowsArgumentNullException() {
        // act & assert
        Assert.Throws<ArgumentNullException>(() => new ValidationException(null!));
    }

    // ─── Constructor(result, message) ────────────────────────────────────────

    [Fact]
    public void Constructor_WithResultAndMessage_SetsMessage() {
        // arrange
        ValidationResult result = Error.Validation("invalid");

        // act
        var exception = new ValidationException(result, "custom message");

        // assert
        Assert.Equal("custom message", exception.Message);
    }

    [Fact]
    public void Constructor_WithResultAndMessage_SetsResult() {
        // arrange
        ValidationResult result = Error.Validation("invalid");

        // act
        var exception = new ValidationException(result, "custom message");

        // assert
        Assert.Same(result, exception.Result);
    }

    // ─── Constructor(result, message, inner) ─────────────────────────────────

    [Fact]
    public void Constructor_WithResultMessageAndInner_SetsInnerException() {
        // arrange
        ValidationResult result = Error.Validation("invalid");
        var inner = new InvalidOperationException("inner");

        // act
        var exception = new ValidationException(result, "outer", inner);

        // assert
        Assert.Same(inner, exception.InnerException);
    }

    [Fact]
    public void Constructor_WithResultMessageAndInner_SetsResult() {
        // arrange
        ValidationResult result = Error.Validation("invalid");
        var inner = new InvalidOperationException("inner");

        // act
        var exception = new ValidationException(result, "outer", inner);

        // assert
        Assert.Same(result, exception.Result);
    }
}
