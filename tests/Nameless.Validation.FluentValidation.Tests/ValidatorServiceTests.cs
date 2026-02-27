using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Nameless.Testing.Tools.Mockers.FluentValidation;
using Nameless.Validation.FluentValidation.Fixtures;

namespace Nameless.Validation.FluentValidation;

public class ValidatorServiceTests {
    private static readonly Dictionary<string, object> EmptyContext = [];

    [Fact]
    public async Task ValidateAsync_Should_Execute_Validator_For_Given_Type() {
        // arrange
        var validator = new ValidatorMocker<Animal>()
                        .WithCanValidateInstancesOfType(true)
                        .WithValidateAsync()
                        .Build();

        var services = new ServiceCollection().AddSingleton(validator);
        var provider = services.BuildServiceProvider();
        var sut = new ValidationService(provider.GetServices<IValidator>());
        var dog = new Animal { Name = "Dog" };

        // act
        var result = await sut.ValidateAsync(
            value: dog,
            context: EmptyContext,
            cancellationToken: CancellationToken.None
        );

        // assert
        Assert.True(result.Success);
    }

    [Fact]
    public async Task ValidateAsync_Should_Log_If_Validator_Not_Found() {
        // arrange
        var sut = new ValidationService([]);
        var dog = new Animal { Name = "Dog" };

        // act
        var result = await sut.ValidateAsync(dog, EmptyContext, CancellationToken.None);

        // assert
        Assert.True(result.Success);
    }

    [Fact]
    public async Task ValidateAsync_Should_Validate_Multiple_Times_With_Same_Result() {
        // arrange
        var validator = new ValidatorMocker<Animal>()
            .WithCanValidateInstancesOfType(true)
            .WithValidateAsync()
            .Build();

        var sut = new ValidationService([validator]);
        var dog = new Animal { Name = "Dog" };

        // act
        var first = await sut.ValidateAsync(dog, EmptyContext, CancellationToken.None);
        var second = await sut.ValidateAsync(dog, EmptyContext, CancellationToken.None);
        var third = await sut.ValidateAsync(dog, EmptyContext, CancellationToken.None);

        // assert
        Assert.True(first.Success && second.Success && third.Success);
    }

    [Fact]
    public async Task WhenValidateAsync_WhenNotProvideDataContext_WhenObjectIsValid_ThenReturnsSuccessfulResult() {
        // arrange
        var validator = new ValidatorMocker<Animal>()
                        .WithCanValidateInstancesOfType(true)
                        .WithValidateAsync()
                        .Build();
        var sut = new ValidationService([validator]);
        var dog = new Animal { Name = "Dog" };

        // act
        var actual = await sut.ValidateAsync(dog, CancellationToken.None);

        // assert
        Assert.True(actual.Success);
    }

    [Fact]
    public async Task WhenValidateAsync_WhenProvidingDataContextWithValues_ThenDataContextShouldBeConsumed() {
        // arrange
        var dataContext = new Dictionary<string, object> {
            { "Value", 123 }
        };
        var validatorMocker = new ValidatorMocker<Animal>()
                              .WithCanValidateInstancesOfType(true)
                              .WithValidateAsync();
        var sut = new ValidationService([validatorMocker.Build()]);
        var dog = new Animal { Name = "Dog" };

        // act
        var actual = await sut.ValidateAsync(dog, dataContext, CancellationToken.None);

        // assert
        Assert.Multiple(() => {
            Assert.True(actual.Success);

            validatorMocker.Verify(mock => mock.ValidateAsync(
                It.Is<IValidationContext>(ctx => ctx.RootContextData.ContainsKey("Value")),
                It.IsAny<CancellationToken>())
            );
        });
    }
}