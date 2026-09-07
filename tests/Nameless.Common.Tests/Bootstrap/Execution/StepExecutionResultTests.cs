using Nameless.Testing.Tools.Attributes;

namespace Nameless.Bootstrap.Execution;

[UnitTest]
public class StepExecutionResultTests {
    [Fact]
    public void Success_WhenExceptionIsNull_ReturnsTrue() {
        // arrange
        var sut = new StepExecutionResult {
            StepName = "TestStep",
            Exception = null
        };

        // act
        var actual = sut.Success;

        // assert
        Assert.True(actual);
    }

    [Fact]
    public void Success_WhenExceptionIsSet_ReturnsFalse() {
        // arrange
        var sut = new StepExecutionResult {
            StepName = "TestStep",
            Exception = new InvalidOperationException("step failed")
        };

        // act
        var actual = sut.Success;

        // assert
        Assert.False(actual);
    }

    [Fact]
    public void Duration_WhenSet_ReturnsCorrectValue() {
        // arrange
        var expected = TimeSpan.FromMilliseconds(123);
        var sut = new StepExecutionResult {
            StepName = "TestStep",
            Duration = expected
        };

        // act
        var actual = sut.Duration;

        // assert
        Assert.Equal(expected, actual);
    }
}
