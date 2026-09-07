using Moq;
using Nameless.Mediator.Requests;
using Nameless.ObjectModel;
using Nameless.Testing.Tools.Attributes;
using Nameless.Testing.Tools.Mockers.Logging;
using Nameless.Validation;

namespace Nameless.Mediator.Pipelines;

[UnitTest]
public class ValidateRequestPipelineBehaviorTests {
    [Fact]
    public async Task HandleAsync_WithNoValidators_CallsNext() {
        // arrange
        var validationServiceMock = new Mock<IValidator>();
        validationServiceMock
            .Setup(svc => svc.ValidateAsync(
                It.IsAny<object>(),
                It.IsAny<IDictionary<string, object>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(ValidationResult.Successful);

        var logger = new LoggerMocker<ValidateRequestPipelineBehavior<ValidatePipelineTestRequest, string>>()
            .WithAnyLogLevel()
            .Build();

        var sut = new ValidateRequestPipelineBehavior<ValidatePipelineTestRequest, string>(
            validationServiceMock.Object,
            logger
        );

        var nextCalled = false;
        RequestHandlerDelegate<string> next = _ => {
            nextCalled = true;
            return Task.FromResult("result");
        };

        // act
        var result = await sut.HandleAsync(new ValidatePipelineTestRequest("Alice"), next, CancellationToken.None);

        // assert
        Assert.Multiple(() => {
            Assert.True(nextCalled);
            Assert.Equal("result", result);
        });
    }

    [Fact]
    public async Task HandleAsync_WithPassingValidator_CallsNext() {
        // arrange
        var validationServiceMock = new Mock<IValidator>();
        validationServiceMock
            .Setup(svc => svc.ValidateAsync(
                It.IsAny<object>(),
                It.IsAny<IDictionary<string, object>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(ValidationResult.Successful);

        var logger = new LoggerMocker<ValidateRequestPipelineBehavior<ValidatePipelineTestRequest, string>>()
            .WithAnyLogLevel()
            .Build();

        var sut = new ValidateRequestPipelineBehavior<ValidatePipelineTestRequest, string>(
            validationServiceMock.Object,
            logger
        );

        var nextCalled = false;
        RequestHandlerDelegate<string> next = _ => {
            nextCalled = true;
            return Task.FromResult("result");
        };

        // act
        var result = await sut.HandleAsync(new ValidatePipelineTestRequest("Bob"), next, CancellationToken.None);

        // assert
        Assert.Multiple(() => {
            Assert.True(nextCalled);
            Assert.Equal("result", result);
        });
    }

    [Fact]
    public async Task HandleAsync_WithFailingValidation_ThrowsValidationException() {
        // arrange
        ValidationResult failingResult = new[] {
            Error.Validation("Name is required.", code: "Name")
        };

        var validationServiceMock = new Mock<IValidator>();
        validationServiceMock
            .Setup(svc => svc.ValidateAsync(
                It.IsAny<object>(),
                It.IsAny<IDictionary<string, object>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(failingResult);

        var logger = new LoggerMocker<ValidateRequestPipelineBehavior<ValidatePipelineTestRequest, string>>()
            .WithAnyLogLevel()
            .Build();

        var sut = new ValidateRequestPipelineBehavior<ValidatePipelineTestRequest, string>(
            validationServiceMock.Object,
            logger
        );

        var nextCalled = false;
        RequestHandlerDelegate<string> next = _ => {
            nextCalled = true;
            return Task.FromResult("result");
        };

        // act
        var exception = await Record.ExceptionAsync(
            () => sut.HandleAsync(new ValidatePipelineTestRequest(Name: null), next, CancellationToken.None)
        );

        // assert
        Assert.Multiple(() => {
            Assert.False(nextCalled);
            Assert.IsType<ValidationException>(exception);
        });
    }
}

// ---------------------------------------------------------------------------
// Test types — declared at namespace level so Castle.DynamicProxy can
// proxy ILogger<T> where T contains this type as a generic argument.
// ---------------------------------------------------------------------------

public record ValidatePipelineTestRequest(string? Name) : IRequest<string>;
