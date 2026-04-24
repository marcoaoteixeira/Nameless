namespace Nameless.ObjectModel;

public class ErrorTests {
    // ─── Parameterless constructor ───────────────────────────────────────────

    [Fact]
    public void DefaultConstructor_ThrowsInvalidOperationException() {
        // act & assert
        Assert.Throws<InvalidOperationException>(() => new Error());
    }

    // ─── Factory methods ─────────────────────────────────────────────────────

    [Fact]
    public void Validation_WithMessage_CreatesErrorWithValidationType() {
        // act
        var error = Error.Validation("invalid input");

        // assert
        Assert.Multiple(() => {
            Assert.Equal(ErrorType.Validation, error.Type);
            Assert.Equal("invalid input", error.Message);
            Assert.Null(error.Code);
        });
    }

    [Fact]
    public void Validation_WithMessageAndCode_IncludesCode() {
        // act
        var error = Error.Validation("invalid input", "VAL001");

        // assert
        Assert.Multiple(() => {
            Assert.Equal(ErrorType.Validation, error.Type);
            Assert.Equal("invalid input", error.Message);
            Assert.Equal("VAL001", error.Code);
        });
    }

    [Fact]
    public void Missing_WithMessage_CreatesErrorWithMissingType() {
        // act
        var error = Error.Missing("not found");

        // assert
        Assert.Multiple(() => {
            Assert.Equal(ErrorType.Missing, error.Type);
            Assert.Equal("not found", error.Message);
            Assert.Null(error.Code);
        });
    }

    [Fact]
    public void Missing_WithMessageAndCode_IncludesCode() {
        // act
        var error = Error.Missing("not found", "NF001");

        // assert
        Assert.Equal("NF001", error.Code);
    }

    [Fact]
    public void Conflict_WithMessage_CreatesErrorWithConflictType() {
        // act
        var error = Error.Conflict("duplicate entry");

        // assert
        Assert.Equal(ErrorType.Conflict, error.Type);
    }

    [Fact]
    public void Conflict_WithMessageAndCode_IncludesCode() {
        // act
        var error = Error.Conflict("duplicate entry", "CONF01");

        // assert
        Assert.Equal("CONF01", error.Code);
    }

    [Fact]
    public void Failure_WithMessage_CreatesErrorWithFailureType() {
        // act
        var error = Error.Failure("something went wrong");

        // assert
        Assert.Equal(ErrorType.Failure, error.Type);
    }

    [Fact]
    public void Failure_WithMessageAndCode_IncludesCode() {
        // act
        var error = Error.Failure("something went wrong", "ERR01");

        // assert
        Assert.Equal("ERR01", error.Code);
    }

    [Fact]
    public void Forbidden_WithMessage_CreatesErrorWithForbiddenType() {
        // act
        var error = Error.Forbidden("access denied");

        // assert
        Assert.Equal(ErrorType.Forbidden, error.Type);
    }

    [Fact]
    public void Forbidden_WithMessageAndCode_IncludesCode() {
        // act
        var error = Error.Forbidden("access denied", "FORB01");

        // assert
        Assert.Equal("FORB01", error.Code);
    }

    [Fact]
    public void Unauthorized_WithMessage_CreatesErrorWithUnauthorizedType() {
        // act
        var error = Error.Unauthorized("not authenticated");

        // assert
        Assert.Equal(ErrorType.Unauthorized, error.Type);
    }

    [Fact]
    public void Unauthorized_WithMessageAndCode_IncludesCode() {
        // act
        var error = Error.Unauthorized("not authenticated", "AUTH01");

        // assert
        Assert.Equal("AUTH01", error.Code);
    }

    // ─── Flatten ─────────────────────────────────────────────────────────────

    [Fact]
    public void Flatten_WithNoCode_ReturnsTypeAndMessage() {
        // arrange
        var error = Error.Validation("invalid value");

        // act
        var flat = error.Flatten;

        // assert
        Assert.Equal("[Validation] invalid value", flat);
    }

    [Fact]
    public void Flatten_WithCode_ReturnsTypeCodeAndMessage() {
        // arrange
        var error = Error.Validation("invalid value", "VAL001");

        // act
        var flat = error.Flatten;

        // assert
        Assert.Equal("[Validation] (VAL001) invalid value", flat);
    }
}
