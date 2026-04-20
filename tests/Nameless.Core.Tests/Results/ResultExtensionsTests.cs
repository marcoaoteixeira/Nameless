using Nameless.ObjectModel;
using Nameless.Results;

namespace Nameless;

public class ResultExtensionsTests {
    // --- Failure ---

    [Fact]
    public void Failure_WhenResultIsSuccess_ReturnsFalse() {
        // arrange
        Result<string> result = "hello";

        // act & assert
        Assert.False(result.Failure);
    }

    [Fact]
    public void Failure_WhenResultHasErrors_ReturnsTrue() {
        // arrange
        Result<string> result = Error.Validation("something wrong");

        // act & assert
        Assert.True(result.Failure);
    }

    [Fact]
    public void Failure_IsOppositeOfSuccess() {
        // arrange
        Result<string> success = "value";
        Result<string> failure = Error.Missing("not found");

        // act & assert
        Assert.Multiple(() => {
            Assert.Equal(!success.Success, success.Failure);
            Assert.Equal(!failure.Success, failure.Failure);
        });
    }
}
