using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Nameless.Validation.FluentValidation.Impl;
using Nameless.Validation.FluentValidation.UnitTest.Fixtures;
using FluentValidationResult = FluentValidation.Results.ValidationResult;

namespace Nameless.Validation.FluentValidation.UnitTest {
    public class ValidatorServiceTests {
        private static Mock<IValidator<T>> CreateValidatorMock<T>(FluentValidationResult? result = null)
            where T : class {
            var validatorMock = new Mock<IValidator<T>>();
            validatorMock
                .Setup(mock => mock.ValidateAsync(It.IsAny<T>(),
                                                  It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(result ?? new FluentValidationResult()));
            validatorMock
                .Setup(mock => mock.CanValidateInstancesOfType(typeof(T)))
                .Returns(true);

            return validatorMock;
        }

        private static Mock<ILogger<ValidationService>> CreateLoggerMock()
            => new();

        [Test]
        public async Task ValidateAsync_Should_Execute_Validator_For_Given_Type() {
            // arrange
            var animalValidatorMock = CreateValidatorMock<Animal>();

            var services = new ServiceCollection();
            services
                .AddSingleton<IValidator>(animalValidatorMock.Object)
                .AddSingleton(animalValidatorMock.Object);

            var provider = services.BuildServiceProvider();
            var sut = new ValidationService(
                validators: provider.GetServices<IValidator>().ToArray(),
                logger: CreateLoggerMock().Object
            );
            var dog = new Animal { Name = "Dog" };

            // act
            var result = await sut.ValidateAsync(value: dog,
                                                 throwOnFailure: false,
                                                 CancellationToken.None);

            // assert
            Assert.That(result.Succeeded, Is.True);
        }

        [Test]
        public async Task ValidateAsync_Should_Log_If_Validator_Not_Found() {
            // arrange
            var loggerMock = CreateLoggerMock();
            var sut = new ValidationService(
                validators: [],
                logger: loggerMock.Object
            );
            var dog = new Animal { Name = "Dog" };

            // act
            var result = await sut.ValidateAsync(value: dog,
                                                 throwOnFailure: false,
                                                 CancellationToken.None);

            // assert
            Assert.That(result.Succeeded, Is.True);
            loggerMock.VerifyInformationCall();
        }

        [Test]
        public async Task ValidateAsync_Should_Validate_Multiple_Times_With_Same_Result() {
            // arrange
            var animalValidatorMock = CreateValidatorMock<Animal>();

            var services = new ServiceCollection();
            services
                .AddSingleton<IValidator>(animalValidatorMock.Object)
                .AddSingleton(animalValidatorMock.Object);

            var provider = services.BuildServiceProvider();
            var sut = new ValidationService(
                validators: provider.GetServices<IValidator>().ToArray(),
                logger: CreateLoggerMock().Object
            );
            var dog = new Animal { Name = "Dog" };

            // act
            var first = await sut.ValidateAsync(value: dog, throwOnFailure: false, CancellationToken.None);
            var second = await sut.ValidateAsync(value: dog, throwOnFailure: false, CancellationToken.None);
            var third = await sut.ValidateAsync(value: dog, throwOnFailure: false, CancellationToken.None);

            // assert
            Assert.That(first.Succeeded && second.Succeeded && third.Succeeded, Is.True);
        }
    }
}