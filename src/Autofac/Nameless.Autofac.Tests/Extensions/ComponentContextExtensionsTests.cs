using Autofac;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Nameless.Mockers;

namespace Nameless.Autofac;

public class ComponentContextExtensionsTests {
    [Test]
    public void WhenGetLogger_WhenNoLoggingConfigured_ThenResolveNullLogger() {
        // arrange
        var builder = new ContainerBuilder();
        using var container = builder.Build();

        // act
        var sut = container.GetLogger(typeof(ComponentContextExtensionsTests));

        // assert
        sut.Should().BeAssignableTo<NullLogger>();
    }

    [Test]
    public void WhenGetLoggerGeneric_WhenNoLoggingConfigured_ThenResolveNullLoggerGeneric() {
        // arrange
        var builder = new ContainerBuilder();
        using var container = builder.Build();

        // act
        var sut = container.GetLogger<ComponentContextExtensionsTests>();

        // assert
        sut.Should().BeAssignableTo<NullLogger<ComponentContextExtensionsTests>>();
    }

    [Test]
    public void WhenGetLogger_WhenLoggingIsConfigured_ThenResolveLogger() {
        // arrange
        var logger = new LoggerMocker<ComponentContextExtensionsTests>()
            .Build();
        var loggerFactory = new LoggerFactoryMocker()
            .WithCreateLogger(logger)
            .Build();
        var builder = new ContainerBuilder();
        builder.RegisterInstance(loggerFactory);

        using var container = builder.Build();

        // act
        var sut = container.GetLogger(typeof(ComponentContextExtensionsTests));

        // assert
        sut.Should()
           .NotBeAssignableTo<NullLogger<ComponentContextExtensionsTests>>()
           .And
           .BeAssignableTo<ILogger<ComponentContextExtensionsTests>>();
    }

    [Test]
    public void WhenGetLoggerGeneric_WhenLoggingIsConfigured_ThenResolveLoggerGeneric() {
        // arrange
        var logger = new LoggerMocker<ComponentContextExtensionsTests>()
            .Build();
        var loggerFactory = new LoggerFactoryMocker()
                            .WithCreateLogger(logger)
                            .Build();
        var builder = new ContainerBuilder();
        builder.RegisterInstance(loggerFactory);

        using var container = builder.Build();

        // act
        var sut = container.GetLogger<ComponentContextExtensionsTests>();

        // assert
        sut.Should()
           .NotBeAssignableTo<NullLogger<ComponentContextExtensionsTests>>()
           .And
           .BeAssignableTo<ILogger<ComponentContextExtensionsTests>>();
    }
}
