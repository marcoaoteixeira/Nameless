using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Nameless.Validation.FluentValidation.Fixtures;
using Nameless.Validation.FluentValidation.Mockers;
using FluentValidationResult = FluentValidation.Results.ValidationResult;

namespace Nameless.Validation.FluentValidation;

public class ValidatorServiceTests {
    private static Mock<IValidator> CreateValidatorMock<T>(FluentValidationResult result = null)
        where T : class {
        var validatorMock = new Mock<IValidator>();
        validatorMock
            .Setup(mock => mock.ValidateAsync(It.IsAny<IValidationContext>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(result ?? new FluentValidationResult()));
        validatorMock
            .Setup(mock => mock.CanValidateInstancesOfType(typeof(T)))
            .Returns(value: true);

        return validatorMock;
    }

    [Fact]
    public async Task ValidateAsync_Should_Execute_Validator_For_Given_Type() {
        // arrange
        var animalValidatorMock = CreateValidatorMock<Animal>();

        var services = new ServiceCollection();
        services
            .AddSingleton(animalValidatorMock.Object)
            .AddSingleton(animalValidatorMock.Object);

        var provider = services.BuildServiceProvider();
        var sut = new ValidationService(provider.GetServices<IValidator>());
        var dog = new Animal { Name = "Dog" };

        // act
        var result = await sut.ValidateAsync(dog,
            new DataContext(),
            CancellationToken.None);

        // assert
        Assert.True(result.Succeeded);
    }

    [Fact]
    public async Task ValidateAsync_Should_Log_If_Validator_Not_Found() {
        // arrange
        var sut = new ValidationService([]);
        var dog = new Animal { Name = "Dog" };

        // act
        var result = await sut.ValidateAsync(dog, [], CancellationToken.None);

        // assert
        Assert.True(result.Succeeded);
    }

    [Fact]
    public async Task ValidateAsync_Should_Validate_Multiple_Times_With_Same_Result() {
        // arrange
        var animalValidatorMock = CreateValidatorMock<Animal>();

        var services = new ServiceCollection();
        services.AddSingleton(animalValidatorMock.Object);

        var provider = services.BuildServiceProvider();
        var sut = new ValidationService(provider.GetServices<IValidator>());
        var dog = new Animal { Name = "Dog" };
        var dataContext = new DataContext();

        // act
        var first = await sut.ValidateAsync(dog, dataContext, CancellationToken.None);
        var second = await sut.ValidateAsync(dog, dataContext, CancellationToken.None);
        var third = await sut.ValidateAsync(dog, dataContext, CancellationToken.None);

        // assert
        Assert.True(first.Succeeded && second.Succeeded && third.Succeeded);
    }

    [Fact]
    public async Task WhenValidateAsync_WhenNotProvideDataContext_WhenObjectIsValid_ThenReturnsSuccessfulResult() {
        // arrange
        var validator = new ValidatorMocker<Animal>()
                        .WithSuccessfulValidateAsync()
                        .Build();
        var sut = new ValidationService([validator]);
        var dog = new Animal { Name = "Dog" };

        // act
        var actual = await sut.ValidateAsync(dog, CancellationToken.None);

        // assert
        Assert.True(actual.Succeeded);
    }

    [Fact]
    public async Task WhenValidateAsync_WhenProvidingDataContextWithValues_ThenDataContextShouldBeConsumed() {
        // arrange
        DataContext dataContext = [
            new KeyValuePair<string, object>(key: "Value", value: 123)
        ];
        var validatorMocker = new ValidatorMocker<Animal>().WithSuccessfulValidateAsync();
        var sut = new ValidationService([validatorMocker.Build()]);
        var dog = new Animal { Name = "Dog" };

        // act
        var actual = await sut.ValidateAsync(dog, dataContext, CancellationToken.None);

        // assert
        Assert.Multiple(() => {
            Assert.True(actual.Succeeded);

            validatorMocker.Verify(mock => mock.ValidateAsync(
                It.Is<IValidationContext>(ctx => ctx.RootContextData.ContainsKey("Value")),
                It.IsAny<CancellationToken>())
            );
        });
    }
}