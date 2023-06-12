using System.Globalization;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Nameless.FileStorage.System;

namespace Nameless.Localization.Json.UnitTests {

    public class TranslationProviderTests {

        [TestCase("pt-BR", "Olá Mundo!")]
        [TestCase("en-US", "Hello World!")]
        [TestCase("es-ES", "¡Hola Mundo!")]
        public async Task TranslationProvider_Can_Return_Translation(string cultureName, string phrase) {
            // arrange
            var fileStorage = new FileStorageImpl(NullApplicationContext.Instance);
            var translationProvider = new TranslationProvider(fileStorage, Options.Create(LocalizationOptions.Default));

            // act
            var translation = await translationProvider.GetAsync(new CultureInfo(cultureName));

            // assert
            translation.Should().NotBeNull();
            translation.Values.Should().NotBeEmpty();
            translation.Values.First().Values.Should().NotBeEmpty();
            translation.Values.First().Values.First().Key.Should().Be("Hello World!");
            translation.Values.First().Values.First().Values.Should().NotBeEmpty();
            translation.Values.First().Values.First().Values.First().Should().Be(phrase);
        }
    }
}
