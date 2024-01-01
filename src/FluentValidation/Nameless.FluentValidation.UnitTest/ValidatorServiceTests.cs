using Autofac;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
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
            return validatorMock;
        }

        private static Mock<ILoggerFactory> CreateLoggerFactoryMock(ILogger? logger = null) {
            var loggerFactoryMock = new Mock<ILoggerFactory>();
            loggerFactoryMock
                .Setup(mock => mock.CreateLogger(It.IsAny<string>()))
                .Returns(logger ?? NullLogger.Instance);
            return loggerFactoryMock;
        }

        private static Mock<ILogger> CreateLoggerMock()
            => new();

        [Test]
        public async Task ValidateAsync_Should_Execute_Validator_For_Given_Type() {
            // arrange
            var animalValidatorMock = CreateValidatorMock<Animal>();
            var loggerFactoryMock = CreateLoggerFactoryMock();

            var builder = new ContainerBuilder();
            builder
                .RegisterInstance(animalValidatorMock.Object)
                .As<IValidator<Animal>>();
            builder
                .RegisterInstance(loggerFactoryMock.Object)
                .As<ILoggerFactory>();

            var container = builder.Build();
            var sut = new ValidatorService(container);
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
            var loggerFactoryMock = CreateLoggerFactoryMock(loggerMock.Object);

            var builder = new ContainerBuilder();
            builder
                .RegisterInstance(loggerFactoryMock.Object)
                .As<ILoggerFactory>();

            var container = builder.Build();
            var sut = new ValidatorService(container);
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
    }
}