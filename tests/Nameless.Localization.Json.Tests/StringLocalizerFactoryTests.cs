using System.Globalization;
using Microsoft.Extensions.Logging;
using Nameless.Localization.Json.Mockers;
using Nameless.Localization.Json.Objects;
using Nameless.Testing.Tools;
using Nameless.Testing.Tools.Mockers;

namespace Nameless.Localization.Json;

public class StringLocalizerFactoryTests {
    private static StringLocalizerFactory CreateSut() {
        const string CultureName = "pt-BR";

        var cultureContext = new CultureProviderMocker().WithCulture(new CultureInfo(CultureName))
                                                        .Build();

        var resource = new Resource(culture: CultureName,
            path: "Path_To_The_Resource",
            messages: [
                new Message("This is a test", "Isso é um teste")
            ],
            isAvailable: true);

        var resourceManager = new ResourceManagerMocker().WithResource(resource)
                                                         .Build();

        var loggerFactory = new LoggerFactoryMocker()
                           .WithCreateLogger(Fast.Mock<ILogger<StringLocalizer>>())
                           .WithCreateLogger(Fast.Mock<ILogger<StringLocalizerFactory>>())
                           .Build();
        var options = Microsoft.Extensions.Options.Options.Create(new LocalizationOptions());

        return new StringLocalizerFactory(
            cultureContext,
            resourceManager,
            options,
            loggerFactory);
    }

    [Fact]
    public void Create_Should_Create_StringLocalizer_Using_Type() {
        // arrange
        var sut = CreateSut();

        // act
        var actual = sut.Create(typeof(StringLocalizerFactoryTests));

        // assert
        Assert.NotNull(actual);
    }

    [Fact]
    public void Create_Should_Create_StringLocalizer_Using_ResourceName_And_ResourcePath() {
        // arrange
        var sut = CreateSut();
        var baseName = typeof(StringLocalizerFactoryTests).Namespace ?? string.Empty;
        const string Location = nameof(StringLocalizerFactoryTests);

        // act
        var actual = sut.Create(baseName, Location);

        // assert
        Assert.NotNull(actual);
    }
}