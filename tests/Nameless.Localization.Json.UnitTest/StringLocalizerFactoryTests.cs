using System.Globalization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Nameless.Localization.Json.Infrastructure;
using Nameless.Localization.Json.Objects;

namespace Nameless.Localization.Json;

public class StringLocalizerFactoryTests {
    private const string CULTURE_NAME = "pt-BR";

    private static StringLocalizerFactory CreateSut(Mock<ICultureContext>? cultureContextMock = null,
                                                    Mock<ITranslationManager>? translationManagerMock = null,
                                                    Mock<ILoggerFactory>? loggerFactoryMock = null)
        => new (cultureContext: (cultureContextMock ?? CreateCultureContextMock()).Object,
                translationManager: (translationManagerMock ?? CreateTranslationManagerMock()).Object,
                loggerFactory: (loggerFactoryMock ?? CreateLoggerFactoryMock()).Object);

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

    private static Mock<ILoggerFactory> CreateLoggerFactoryMock() {
        var result = new Mock<ILoggerFactory>();

        result
            .Setup(mock => mock.CreateLogger(It.IsAny<string>()))
            .Returns(NullLogger<StringLocalizer>.Instance);

        return result;
    }

    [Test]
    public void Create_Should_Create_StringLocalizer_Using_Type() {
        // arrange
        var cultureContextMock = CreateCultureContextMock();
        var translationManagerMock = CreateTranslationManagerMock();
        var sut = CreateSut(cultureContextMock, translationManagerMock);

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