using Nameless.ObjectModel;

namespace Nameless;

public class ErrorExtensionsTests {
    // --- Flatten ---

    [Fact]
    public void Flatten_WithMultipleErrors_JoinsBySemicolon() {
        // arrange
        var errors = new[] {
            Error.Validation("field is required", "REQUIRED"),
            Error.Missing("item not found", "NOT_FOUND")
        };

        // act
        var flat = errors.Flatten();

        // assert
        Assert.Multiple(() => {
            Assert.Contains("[Validation]", flat);
            Assert.Contains("[Missing]", flat);
            Assert.Contains(";", flat);
        });
    }

    [Fact]
    public void Flatten_WithSingleError_ReturnsSingleErrorString() {
        // arrange
        var errors = new[] { Error.Failure("oops") };

        // act
        var flat = errors.Flatten();

        // assert
        Assert.DoesNotContain(";", flat);
        Assert.Contains("[Failure]", flat);
        Assert.Contains("oops", flat);
    }

    [Fact]
    public void Flatten_WithEmptyCollection_ReturnsEmpty() {
        // arrange
        var errors = Array.Empty<Error>();

        // act
        var flat = errors.Flatten();

        // assert
        Assert.Equal(string.Empty, flat);
    }

    [Fact]
    public void Flatten_WithCodedError_IncludesCodeInOutput() {
        // arrange
        var errors = new[] { Error.Validation("invalid", "ERR_001") };

        // act
        var flat = errors.Flatten();

        // assert
        Assert.Contains("ERR_001", flat);
    }
}
