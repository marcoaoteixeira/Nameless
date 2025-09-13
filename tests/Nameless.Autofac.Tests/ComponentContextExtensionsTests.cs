#pragma warning disable IDE0079 // Shut up you fucker...

using Autofac;
using Microsoft.Extensions.Logging.Abstractions;
using Nameless.Testing.Tools.Mockers.Logging;

namespace Nameless.Autofac;

public class ComponentContextExtensionsTests {
    [Fact]
    public void WhenGetLogger_WhenNoLoggingConfigured_ThenResolveNullLogger() {
        // arrange
        var builder = new ContainerBuilder();
        using var container = builder.Build();

        // act
#pragma warning disable CA2263
        var sut = container.GetLogger(typeof(ComponentContextExtensionsTests));
#pragma warning restore CA2263

        // assert
        Assert.IsType<NullLogger>(sut);
    }

    [Fact]
    public void WhenGetLoggerGeneric_WhenNoLoggingConfigured_ThenResolveNullLoggerGeneric() {
        // arrange
        var builder = new ContainerBuilder();
        using var container = builder.Build();

        // act
        var sut = container.GetLogger<ComponentContextExtensionsTests>();

        // assert
        Assert.IsType<NullLogger<ComponentContextExtensionsTests>>(sut);
    }

    [Fact]
    public void WhenGetLogger_WhenLoggingIsConfigured_ThenResolveLogger() {
        // arrange
        var logger = new LoggerMocker<ComponentContextExtensionsTests>().Build();
        var categoryName = typeof(ComponentContextExtensionsTests).FullName ??
                           nameof(ComponentContextExtensionsTests);
        var loggerFactory = new LoggerFactoryMocker().WithCreateLogger(categoryName, logger).Build();
        var builder = new ContainerBuilder();
        builder.RegisterInstance(loggerFactory);

        using var container = builder.Build();

        // act
#pragma warning disable CA2263
        var sut = container.GetLogger(typeof(ComponentContextExtensionsTests));
#pragma warning restore CA2263

        // assert
        Assert.IsNotType<NullLogger<ComponentContextExtensionsTests>>(sut);
    }

    [Fact]
    public void WhenGetLoggerGeneric_WhenLoggingIsConfigured_ThenResolveLoggerGeneric() {
        // arrange
        var logger = new LoggerMocker<ComponentContextExtensionsTests>().Build();
        var categoryName = typeof(ComponentContextExtensionsTests).FullName ??
                           nameof(ComponentContextExtensionsTests);
        var loggerFactory = new LoggerFactoryMocker().WithCreateLogger(categoryName, logger).Build();
        var builder = new ContainerBuilder();
        builder.RegisterInstance(loggerFactory);

        using var container = builder.Build();

        // act
        var sut = container.GetLogger<ComponentContextExtensionsTests>();

        // assert
        Assert.IsNotType<NullLogger<ComponentContextExtensionsTests>>(sut);
    }
}