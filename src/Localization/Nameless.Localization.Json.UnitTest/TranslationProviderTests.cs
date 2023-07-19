using System.Globalization;
using FluentAssertions;
using Nameless.FileStorage.System;

namespace Nameless.Localization.Json.UnitTests {

    public class TranslationProviderTests {

        [TestCase("pt-BR", "Olá Mundo!")]
        [TestCase("en-US", "Hello World!")]
        [TestCase("es-ES", "¡Hola Mundo!")]
        public async Task TranslationProvider_Can_Return_Translation(string cultureName, string phrase) {
            // arrange
            var fileStorage = new FileStorageImpl(NullApplicationContext.Instance);
            var translationProvider = new TranslationProvider(fileStorage, LocalizationOptions.Default);

            // act
            var translation = await translationProvider.GetAsync(new CultureInfo(cultureName));

            // assert
            translation.Should().NotBeNull();
            translation.EntryCollections.Should().NotBeEmpty();
            translation.EntryCollections.First().Entries.Should().NotBeEmpty();
            translation.EntryCollections.First().Entries.First().Key.Should().Be("Hello World!");
            translation.EntryCollections.First().Entries.First().Values.Should().NotBeEmpty();
            translation.EntryCollections.First().Entries.First().Values.First().Should().Be(phrase);
        }
    }
}
