using Moq;
using Nameless.Mediator.Streams;
using Nameless.ObjectModel;
using Nameless.Testing.Tools.Mockers.Logging;
using Nameless.Validation;

namespace Nameless.Mediator.Pipelines;

public record ValidateStreamTestRequest(string Name) : IStream<string>;

[UnitTest]
public class ValidateStreamPipelineBehaviorTests {
    private static ValidateStreamPipelineBehavior<ValidateStreamTestRequest, string> CreateSut(ValidationResult result, Mock<IValidator>? validator = null) {
        validator ??= new Mock<IValidator>();
        validator.Setup(v => v.ValidateAsync(It.IsAny<object>(), It.IsAny<IDictionary<string, object>>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(result);

        var logger = new LoggerMocker<ValidateStreamPipelineBehavior<ValidateStreamTestRequest, string>>().WithAnyLogLevel().Build();

        return new ValidateStreamPipelineBehavior<ValidateStreamTestRequest, string>(validator.Object, logger);
    }

    private static async IAsyncEnumerable<string> Items(params string[] items) {
        foreach (var item in items) {
            await Task.Yield();

            yield return item;
        }
    }

    [Fact]
    public async Task HandleAsync_WhenValid_YieldsAllItemsWithoutThrowing() {
        // arrange
        var validator = new Mock<IValidator>();
        var sut = CreateSut(ValidationResult.Successful, validator);
        var result = new List<string>();

        // act
        var exception = await Record.ExceptionAsync(async () => {
            await foreach (var item in sut.HandleAsync(new ValidateStreamTestRequest("a"), () => Items("x", "y"), CancellationToken.None)) {
                result.Add(item);
            }
        });

        // assert
        Assert.Multiple(
            () => Assert.Null(exception),
            () => Assert.Equal(["x", "y"], result),
            () => validator.Verify(v => v.ValidateAsync(It.IsAny<object>(), It.IsAny<IDictionary<string, object>>(), It.IsAny<CancellationToken>()), Times.Once)
        );
    }

    [Fact]
    public async Task HandleAsync_WhenInvalid_ThrowsValidationExceptionAndSkipsNext() {
        // arrange
        var sut = CreateSut(new[] { Error.Validation("bad", "E1") });
        var nextCalled = false;

        // act & assert
        await Assert.ThrowsAsync<ValidationException>(async () => {
            await foreach (var _ in sut.HandleAsync(new ValidateStreamTestRequest("a"), () => { nextCalled = true; return Items("x"); }, CancellationToken.None)) { }
        });
        Assert.False(nextCalled);
    }
}
