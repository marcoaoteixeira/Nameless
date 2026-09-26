namespace Nameless.ObjectModel;

[UnitTest]
public class ErrorExtensionsTests {
    // --- Flatten ---

    [Fact]
    public void Flatten_WithMultipleErrors_JoinsBySemicolon() {
        // arrange
        var errors = new[] {
            Error.Validation("Validation 1", "Code 1"),
            Error.Missing("Missing 1", "Code 1"),
            Error.Validation("Validation 2"),
            Error.Missing("Missing 2"),
        };
        const string Expected = "[Validation] (Code 1) Validation 1; Validation 2 | [Missing] (Code 1) Missing 1; Missing 2";

        // act
        var actual = errors.Flatten();

        // assert
        Assert.Equal(Expected, actual);
    }

    [Fact]
    public void Flatten_WithSingleError_ReturnsSingleErrorString() {
        // arrange
        var errors = new[] { Error.Failure("oops") };

        // act
        var flat = errors.Flatten();

        // assert
        Assert.Multiple(
            () => Assert.DoesNotContain(";", flat),
            () => Assert.Contains("[Failure]", flat),
            () => Assert.Contains("oops", flat)
        );
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
