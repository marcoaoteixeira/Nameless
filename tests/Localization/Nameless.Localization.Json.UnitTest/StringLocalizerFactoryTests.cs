using System.Globalization;
using Nameless.Localization.Json.Mockers;
using Nameless.Localization.Json.Objects;
using Nameless.Mockers;

namespace Nameless.Localization.Json;

public class StringLocalizerFactoryTests {
    private static StringLocalizerFactory CreateSut() {
        const string cultureName = "pt-BR";

        var cultureContextMocker = new CultureContextMocker()
            .WithCulture(new CultureInfo(cultureName));

        var translation = new Translation(culture: cultureName,
                                          regions: [
                                              new Region(name: $"[{typeof(StringLocalizerFactoryTests).Assembly.GetName().Name}] {typeof(StringLocalizerFactoryTests).FullName}",
                                                         messages: [
                                                             new Message("This is a test", "Isso é um teste")
                                                         ])
                                          ]);
        var translationManagerMocker = new TranslationManagerMocker()
            .WithTranslation(translation);

        var loggerMocker = new LoggerMocker<StringLocalizerFactory>();
        var loggerForStringLocalizerMocker = new LoggerMocker<StringLocalizer>();

        return new StringLocalizerFactory(cultureContextMocker.Build(),
                                          translationManagerMocker.Build(),
                                          loggerMocker.Build(),
                                          loggerForStringLocalizerMocker.Build());
    }

    [Test]
    public void Create_Should_Create_StringLocalizer_Using_Type() {
        // arrange
        var sut = CreateSut();

        // act
        var actual = sut.Create(typeof(StringLocalizerFactoryTests));

        // assert
        Assert.That(actual, Is.Not.Null);
    }

    [Test]
    public void Create_Should_Create_StringLocalizer_Using_ResourceName_And_ResourcePath() {
        // arrange
        var sut = CreateSut();
        var resourceName = typeof(StringLocalizerFactoryTests)
                           .Assembly
                           .GetName()
                           .Name;
        var resourcePath = typeof(StringLocalizerFactoryTests)
            .FullName;

        // act
        var actual = sut.Create(resourceName, resourcePath);

        // assert
        Assert.That(actual, Is.Not.Null);
    }
}