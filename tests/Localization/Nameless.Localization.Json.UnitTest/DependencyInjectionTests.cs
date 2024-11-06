using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Nameless.Localization.Json.Infrastructure;
using Nameless.Localization.Json.Infrastructure.Impl;
using Nameless.Mockers;

namespace Nameless.Localization.Json;

public class DependencyInjectionTests {
    [Test]
    public void Register_Resolve_Service() {
        // arrange
        var services = new ServiceCollection();
        services.AddJsonLocalization(_ => { });

        // We need an IFileProvider
        var fileProviderMock = new Mock<IFileProvider>();
        fileProviderMock
            .Setup(mock => mock.GetFileInfo(It.IsAny<string>()))
            .Returns(Mock.Of<IFileInfo>());
        services.AddSingleton(fileProviderMock.Object);
        services.AddSingleton(new LoggerMocker<CultureContext>().Build());
        services.AddSingleton(new LoggerMocker<TranslationManager>().Build());
        services.AddSingleton(new LoggerMocker<StringLocalizer>().Build());
        services.AddSingleton(new LoggerMocker<StringLocalizerFactory>().Build());

        using var container = services.BuildServiceProvider();

        // act
        var factory = container.GetService<IStringLocalizerFactory>();
        var localizer = container.GetService<IStringLocalizer<Fake>>();

        // assert
        Assert.Multiple(() => {
            Assert.That(factory, Is.InstanceOf<StringLocalizerFactory>());
            Assert.That(localizer, Is.InstanceOf<StringLocalizer<Fake>>());
        });
    }

    public class Fake;
}