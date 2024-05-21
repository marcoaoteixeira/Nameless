using Autofac;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using Nameless.FluentValidation.Fixtures;
using Nameless.FluentValidation.Impl;

namespace Nameless.FluentValidation {
    public class ValidatorServiceTests {
        private static Mock<IValidator<T>> CreateValidatorMock<T>(ValidationResult? result = null)
            where T : class {
            var validatorMock = new Mock<IValidator<T>>();
            validatorMock
                .Setup(mock => mock.ValidateAsync(
                    It.IsAny<T>(),
                    It.IsAny<CancellationToken>())
                )
                .Returns(Task.FromResult(result ?? new ValidationResult()));
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

            var builder = new ContainerBuilder();
            builder
                .RegisterInstance(animalValidatorMock.Object)
                .As<IValidator<Animal>>()
                .As<IValidator>();

            var container = builder.Build();
            var sut = new ValidationService(
                validators: container.Resolve<IValidator[]>(),
                logger: CreateLoggerMock().Object
            );
            var dog = new Animal { Name = "Dog" };

            // act
            var result = await sut.ValidateAsync(dog, CancellationToken.None);

            // assert
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public async Task ValidateAsync_Should_Log_If_Validator_Not_Found() {
            // arrange
            var loggerMock = CreateLoggerMock();

            var builder = new ContainerBuilder();

            var container = builder.Build();
            var sut = new ValidationService(
                validators: [],
                logger: loggerMock.Object
            );
            var dog = new Animal { Name = "Dog" };

            // act
            var result = await sut.ValidateAsync(dog, CancellationToken.None);

            // assert
            Assert.That(result.IsValid, Is.True);
            loggerMock
                .Verify(mock => mock.Log(
                    It.Is<LogLevel>(logLevel => logLevel == LogLevel.Information),
                    It.Is<EventId>(eventId => eventId.Id == 0),
                    It.Is<It.IsAnyType>((obj, type) => type.Name == "FormattedLogValues"),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()
                ));
        }

        [Test]
        public async Task ValidateAsync_Should_Validate_Multiple_Times_With_Same_Result() {
            // arrange
            var animalValidatorMock = CreateValidatorMock<Animal>();

            var builder = new ContainerBuilder();
            builder
                .RegisterInstance(animalValidatorMock.Object)
                .As<IValidator<Animal>>()
                .As<IValidator>();

            var container = builder.Build();
            var sut = new ValidationService(
                validators: container.Resolve<IValidator[]>(),
                logger: CreateLoggerMock().Object
            );
            var dog = new Animal { Name = "Dog" };

            // act
            var first = await sut.ValidateAsync(dog, CancellationToken.None);
            var second = await sut.ValidateAsync(dog, CancellationToken.None);
            var third = await sut.ValidateAsync(dog, CancellationToken.None);

            // assert
            Assert.That(first.IsValid && second.IsValid && third.IsValid, Is.True);
        }
    }
}