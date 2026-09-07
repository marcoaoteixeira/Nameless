using FluentValidation;
using Nameless.Testing.Tools.Attributes;

namespace Nameless.Validation.FluentValidation;

[UnitTest]
public class FluentValidationValidatorTests {
    private sealed record SampleModel(string Name, int Age);

    // Validator that always passes
    private sealed class PassingValidator : AbstractValidator<SampleModel> {
        public PassingValidator() {
            RuleFor(x => x.Name).NotEmpty();
        }
    }

    // Validator that always fails with two errors
    private sealed class FailingNameValidator : AbstractValidator<SampleModel> {
        public FailingNameValidator() {
            RuleFor(x => x.Name)
                .Must(_ => false)
                .WithMessage("Name validation failed");
        }
    }

    // Validator that always fails with one error on Age
    private sealed class FailingAgeValidator : AbstractValidator<SampleModel> {
        public FailingAgeValidator() {
            RuleFor(x => x.Age)
                .Must(_ => false)
                .WithMessage("Age validation failed");
        }
    }

    // Validator that captures context data for verification
    private sealed class ContextCapturingValidator : AbstractValidator<SampleModel> {
        private readonly Dictionary<string, object> _capturedContext;

        public ContextCapturingValidator(Dictionary<string, object> capturedContext) {
            _capturedContext = capturedContext;

            RuleFor(x => x.Name).Custom((_, ctx) => {
                foreach (var pair in ctx.RootContextData) {
                    _capturedContext[pair.Key] = pair.Value;
                }
            });
        }
    }

    [Fact]
    public async Task ValidateAsync_WithNoValidators_ReturnsSuccess() {
        // arrange
        var sut = new FluentValidationValidator(validators: []);
        var model = new SampleModel("Alice", 30);

        // act
        var actual = await sut.ValidateAsync(model, new Dictionary<string, object>(), CancellationToken.None);

        // assert
        Assert.True(actual.Success);
    }

    [Fact]
    public async Task ValidateAsync_WithPassingValidator_ReturnsSuccess() {
        // arrange
        var sut = new FluentValidationValidator(validators: [new PassingValidator()]);
        var model = new SampleModel("Alice", 30);

        // act
        var actual = await sut.ValidateAsync(model, new Dictionary<string, object>(), CancellationToken.None);

        // assert
        Assert.True(actual.Success);
    }

    [Fact]
    public async Task ValidateAsync_WithFailingValidator_ReturnsFailureWithErrors() {
        // arrange
        var sut = new FluentValidationValidator(validators: [new FailingNameValidator()]);
        var model = new SampleModel("Alice", 30);

        // act
        var actual = await sut.ValidateAsync(model, new Dictionary<string, object>(), CancellationToken.None);

        // assert
        Assert.Multiple(() => {
            Assert.False(actual.Success);
            Assert.NotEmpty(actual.Errors);
            Assert.Equal("Name validation failed", actual.Errors[0].Message);
        });
    }

    [Fact]
    public async Task ValidateAsync_WithMultipleValidators_AggregatesAllErrors() {
        // arrange
        var sut = new FluentValidationValidator(validators: [
            new FailingNameValidator(),
            new FailingAgeValidator()
        ]);
        var model = new SampleModel("Alice", 30);

        // act
        var actual = await sut.ValidateAsync(model, new Dictionary<string, object>(), CancellationToken.None);

        // assert — each validator contributes at least one error; both messages must be present
        Assert.Multiple(() => {
            Assert.False(actual.Success);
            Assert.True(actual.Errors.Length >= 2);
            Assert.Contains(actual.Errors, e => e.Message == "Name validation failed");
            Assert.Contains(actual.Errors, e => e.Message == "Age validation failed");
        });
    }

    [Fact]
    public async Task ValidateAsync_WithCustomContext_PassesContextToValidator() {
        // arrange
        var capturedContext = new Dictionary<string, object>();
        var sut = new FluentValidationValidator(validators: [new ContextCapturingValidator(capturedContext)]);
        var model = new SampleModel("Alice", 30);
        var inputContext = new Dictionary<string, object> {
            ["TenantId"] = "tenant-001",
            ["UserId"] = 42
        };

        // act
        await sut.ValidateAsync(model, inputContext, CancellationToken.None);

        // assert
        Assert.Multiple(() => {
            Assert.True(capturedContext.ContainsKey("TenantId"));
            Assert.Equal("tenant-001", capturedContext["TenantId"]);
            Assert.True(capturedContext.ContainsKey("UserId"));
            Assert.Equal(42, capturedContext["UserId"]);
        });
    }
}
