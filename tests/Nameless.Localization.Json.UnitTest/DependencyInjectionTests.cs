using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Nameless.Localization.Json.Infrastructure.Impl;

namespace Nameless.Localization.Json;

public class DependencyInjectionTests {
    [Test]
    public void Register_Resolve_Service() {
        // arrange
        var services = new ServiceCollection();
        services.AddJsonLocalization();

        // We need an IFileProvider
        var fileProviderMock = new Mock<IFileProvider>();
        fileProviderMock
            .Setup(mock => mock.GetFileInfo(It.IsAny<string>()))
            .Returns(Mock.Of<IFileInfo>());
        services.AddSingleton(fileProviderMock.Object);
        services.AddSingleton((ILogger<CultureContext>)NullLogger<CultureContext>.Instance);
        services.AddSingleton((ILogger<TranslationManager>)NullLogger<TranslationManager>.Instance);
        services.AddSingleton((ILogger<StringLocalizer>)NullLogger<StringLocalizer>.Instance);
        services.AddSingleton((ILoggerFactory)NullLoggerFactory.Instance);

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