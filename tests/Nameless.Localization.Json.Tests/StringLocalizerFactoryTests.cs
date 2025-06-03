using System.Globalization;
using Nameless.Localization.Json.Mockers;
using Nameless.Localization.Json.Objects;
using Nameless.Testing.Tools.Mockers;

namespace Nameless.Localization.Json;

public class StringLocalizerFactoryTests {
    private static StringLocalizerFactory CreateSut() {
        const string cultureName = "pt-BR";

        var cultureContext = new CultureProviderMocker().WithCulture(new CultureInfo(cultureName))
                                                        .Build();

        var resource = new Resource(culture: cultureName,
            path: "Path_To_The_Resource",
            messages: [
                new Message("This is a test", "Isso é um teste")
            ],
            isAvailable: true);

        var resourceManager = new ResourceManagerMocker().WithResource(resource)
                                                         .Build();

        var loggerFactory = new LoggerFactoryMocker().Build();
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
        Assert.That(actual, Is.Not.Null);
    }

    [Fact]
    public void Create_Should_Create_StringLocalizer_Using_ResourceName_And_ResourcePath() {
        // arrange
        var sut = CreateSut();
        var baseName = typeof(StringLocalizerFactoryTests).Namespace ?? string.Empty;
        const string location = nameof(StringLocalizerFactoryTests);

        // act
        var actual = sut.Create(baseName, location);

        // assert
        Assert.That(actual, Is.Not.Null);
    }
}