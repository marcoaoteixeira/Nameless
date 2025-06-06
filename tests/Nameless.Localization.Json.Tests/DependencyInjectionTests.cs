using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Moq;
using Nameless.Localization.Json.Infrastructure;
using Nameless.Testing.Tools;
using Nameless.Testing.Tools.Mockers;

namespace Nameless.Localization.Json;

public class DependencyInjectionTests {
    [Fact]
    public void Register_Resolve_Service() {
        // arrange
        var services = new ServiceCollection();
        services.RegisterLocalizationServices(_ => { });

        // We need an IFileProvider
        var fileProviderMock = new Mock<IFileProvider>();
        fileProviderMock
           .Setup(mock => mock.GetFileInfo(It.IsAny<string>()))
           .Returns(Mock.Of<IFileInfo>());

        var loggerFactory = new LoggerFactoryMocker()
                           .WithCreateLogger(Quick.Mock<ILogger<StringLocalizer>>())
                           .WithCreateLogger(Quick.Mock<ILogger<StringLocalizerFactory>>())
                           .Build();

        services.AddSingleton(fileProviderMock.Object);
        services.AddSingleton(new LoggerMocker<CultureProvider>().Build());
        services.AddSingleton(new LoggerMocker<ResourceManager>().Build());
        services.AddSingleton(loggerFactory);

        using var container = services.BuildServiceProvider();

        // act
        var factory = container.GetService<IStringLocalizerFactory>();
        var localizer = container.GetService<IStringLocalizer<Fake>>();

        // assert
        Assert.Multiple(() => {
            Assert.IsType<StringLocalizerFactory>(factory);
            Assert.IsType<StringLocalizer<Fake>>(localizer);
        });
    }

    public class Fake;
}