using System.Globalization;
using Moq;
using Nameless.Localization.Json.Infrastructure;
using Nameless.Localization.Json.Objects;

namespace Nameless.Localization.Json;

public class StringLocalizerFactoryTests {
    private const string CULTURE_NAME = "pt-BR";

    private static Mock<ICultureContext> CreateCultureContextMock() {
        var result = new Mock<ICultureContext>();

        result
            .Setup(mock => mock.GetCurrentCulture())
            .Returns(new CultureInfo(CULTURE_NAME));

        return result;
    }

    private static Mock<ITranslationManager> CreateTranslationManagerMock() {
        var result = new Mock<ITranslationManager>();
        var translation = new Translation(CULTURE_NAME, [
            new Region ($"[{typeof(StringLocalizerFactoryTests).Assembly.GetName().Name}] {typeof(StringLocalizerFactoryTests).FullName}", [
                new Message("This is a test", "Isso é um teste")
            ])
        ]);

        result
            .Setup(mock => mock.GetTranslation(translation.Culture))
            .Returns(translation);

        return result;
    }

    [Test]
    public void Create_Should_Create_StringLocalizer_Using_Type() {
        // arrange
        var cultureContextMock = CreateCultureContextMock();
        var translationManagerMock = CreateTranslationManagerMock();
        var sut = new StringLocalizerFactory(
            cultureContext: cultureContextMock.Object,
            translationManager: translationManagerMock.Object
        );

        // act
        var actual = sut.Create(typeof(StringLocalizerFactoryTests));

        // assert
        Assert.That(actual, Is.Not.Null);
    }

    [Test]
    public void Create_Should_Create_StringLocalizer_Using_ResourceName_And_ResourcePath() {
        // arrange
        var cultureContextMock = CreateCultureContextMock();
        var translationManagerMock = CreateTranslationManagerMock();
        var sut = new StringLocalizerFactory(
            cultureContext: cultureContextMock.Object,
            translationManager: translationManagerMock.Object
        );
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