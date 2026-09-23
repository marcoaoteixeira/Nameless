using Moq;
using Nameless.ObjectModel;
using Nameless.Testing.Tools.Mockers.Logging;
using Nameless.Validation;

namespace Nameless.Mediator.Pipelines;

public record ValidateVoidTestRequest(string? Name) : Requests.IRequest;

[UnitTest]
public class ValidateVoidRequestPipelineBehaviorTests {
    private static ValidateRequestPipelineBehavior<ValidateVoidTestRequest> CreateSut(ValidationResult result) {
        var validator = new Mock<IValidator>();
        validator.Setup(v => v.ValidateAsync(It.IsAny<object>(), It.IsAny<IDictionary<string, object>>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(result);

        var logger = new LoggerMocker<ValidateRequestPipelineBehavior<ValidateVoidTestRequest>>().WithAnyLogLevel().Build();

        return new ValidateRequestPipelineBehavior<ValidateVoidTestRequest>(validator.Object, logger);
    }

    [Fact]
    public async Task HandleAsync_WhenValid_CallsNext() {
        // arrange
        var sut = CreateSut(ValidationResult.Successful);
        var nextCalled = false;

        // act
        await sut.HandleAsync(new ValidateVoidTestRequest("a"), _ => { nextCalled = true; return Task.CompletedTask; }, CancellationToken.None);

        // assert
        Assert.True(nextCalled);
    }

    [Fact]
    public async Task HandleAsync_WhenInvalid_ThrowsValidationExceptionAndSkipsNext() {
        // arrange
        var sut = CreateSut(new[] { Error.Validation("bad", "E1") });
        var nextCalled = false;

        // act & assert
        await Assert.ThrowsAsync<ValidationException>(
            () => sut.HandleAsync(new ValidateVoidTestRequest(null), _ => { nextCalled = true; return Task.CompletedTask; }, CancellationToken.None));
        Assert.False(nextCalled);
    }
}
