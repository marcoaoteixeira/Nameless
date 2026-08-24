using Nameless.Bootstrap.Execution;
using Nameless.Testing.Tools.Attributes;

namespace Nameless.Bootstrap;

[UnitTest]
public class BootstrapExceptionTests {
    [Fact]
    public void Constructor_Default_SetsDefaultMessage() {
        // arrange & act
        var sut = new BootstrapException();

        // assert
        Assert.Equal("An error occurred while executing the bootstrapper", sut.Message);
        Assert.Empty(sut.Results);
    }

    [Fact]
    public void Constructor_WithMessage_SetsMessage() {
        // arrange
        const string Message = "custom bootstrap error";

        // act
        var sut = new BootstrapException(Message);

        // assert
        Assert.Equal(Message, sut.Message);
        Assert.Empty(sut.Results);
    }

    [Fact]
    public void Constructor_WithResults_SetsResults() {
        // arrange
        var results = new[] {
            new StepExecutionResult {
                StepName = "Step1",
                StartTime = DateTimeOffset.UtcNow,
                Duration = TimeSpan.FromMilliseconds(50),
                Exception = new InvalidOperationException("step failed")
            }
        };

        // act
        var sut = new BootstrapException(results);

        // assert
        Assert.Multiple(() => {
            Assert.Single(sut.Results);
            Assert.Equal("Step1", sut.Results[0].StepName);
            Assert.False(sut.Results[0].Success);
        });
    }

    [Fact]
    public void Constructor_WithMessageAndResults_SetsBoth() {
        // arrange
        const string Message = "pipeline failed";
        var results = new[] {
            new StepExecutionResult {
                StepName = "StepA",
                StartTime = DateTimeOffset.UtcNow,
                Duration = TimeSpan.Zero
            }
        };

        // act
        var sut = new BootstrapException(Message, results);

        // assert
        Assert.Equal(Message, sut.Message);
        Assert.Single(sut.Results);
    }

    [Fact]
    public void Constructor_WithMessageResultsAndInnerException_SetsAll() {
        // arrange
        const string Message = "wrapped error";
        var inner = new IOException("disk error");
        var results = new[] {
            new StepExecutionResult {
                StepName = "StepB",
                StartTime = DateTimeOffset.UtcNow,
                Duration = TimeSpan.FromSeconds(1),
                Exception = inner
            }
        };

        // act
        var sut = new BootstrapException(Message, results, inner);

        // assert
        Assert.Multiple(() => {
            Assert.Equal(Message, sut.Message);
            Assert.Same(inner, sut.InnerException);
            Assert.Single(sut.Results);
            Assert.Equal("StepB", sut.Results[0].StepName);
        });
    }

    [Fact]
    public void Constructor_WithEmptyResults_ResultsIsEmpty() {
        // arrange & act
        var sut = new BootstrapException([]);

        // assert
        Assert.Empty(sut.Results);
    }
}
