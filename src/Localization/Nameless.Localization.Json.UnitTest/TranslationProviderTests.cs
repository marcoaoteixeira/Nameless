using System.Globalization;
using FluentAssertions;
using Nameless.FileStorage.System;
using Nameless.Localization.Json.Impl;

namespace Nameless.Localization.Json.UnitTest {

    public class TranslationProviderTests {

        [TestCase("pt-BR", "Olá Mundo!")]
        [TestCase("en-US", "Hello World!")]
        [TestCase("es-ES", "¡Hola Mundo!")]
        public async Task TranslationProvider_Can_Return_Translation(string cultureName, string phrase) {
            // arrange
            var fileStorage = new FileStorageImpl(NullApplicationContext.Instance);
            var translationProvider = new FileTranslationProvider(fileStorage, LocalizationOptions.Default);

            // act
            var translation = await translationProvider.GetAsync(new CultureInfo(cultureName));

            // assert
            translation.Should().NotBeNull();
            translation.Should().NotBeEmpty();
            translation.First().Should().NotBeEmpty();
            translation.First().First().Key.Should().Be("Hello World!");
            translation.First().First().Value.Should().Be(phrase);
        }
    }
}
