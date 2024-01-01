using System.Diagnostics;
using System.Globalization;
using Microsoft.Extensions.Localization;
using Nameless.Localization.Microsoft.Json.Objects;

namespace Nameless.Localization.Microsoft.Json {
    public class StringLocalizerTests {
        [Test]
        public void Indexer_Should_Return_LocalizedString() {
            // arrange
            const string messageID = "This is a test";
            const string expected = "Isso é um teste";
            var culture = new CultureInfo("pt-BR");
            const string resourceName = "AssemblyName";
            const string resourcePath = "ClassName";
            var region = new Region {
                Name = $"[{resourceName}] {resourcePath}",
                Messages = [
                    new() { ID = messageID, Text = expected }
                ]
            };
            var sut = new StringLocalizer(
                culture,
                resourceName,
                resourcePath,
                region,
                (c, rn, np) => NullStringLocalizer.Instance
            );

            // act
            var actual = sut[messageID];

            // assert
            Assert.That(actual.Value, Is.EqualTo(expected)); ;
        }

        [Test]
        public void Indexer_Should_Return_LocalizedString_With_Parameters() {
            // arrange
            const string messageID = "This is a test {0}, {1}, {2}, {3}";
            const string expected = "Isso é um teste 1, 2, 3, 4";
            var culture = new CultureInfo("pt-BR");
            const string resourceName = "AssemblyName";
            const string resourcePath = "ClassName";
            var args = new object[] { 1, 2, 3, 4 };
            var region = new Region {
                Name = $"[{resourceName}] {resourcePath}",
                Messages = [
                    new() { ID = messageID, Text = expected }
                ]
            };
            var sut = new StringLocalizer(
                culture,
                resourceName,
                resourcePath,
                region,
                (c, rn, np) => NullStringLocalizer.Instance
            );

            // act
            var actual = sut[messageID, args];

            // assert
            Assert.That(actual.Value, Is.EqualTo(expected)); ;
        }

        [Test]
        public void GetAllStrings_Should_Return_All_Messages() {
            // arrange
            var culture = new CultureInfo("pt-BR");
            const string resourceName = "AssemblyName";
            const string resourcePath = "ClassName";
            var region = new Region {
                Name = $"[{resourceName}] {resourcePath}",
                Messages = [
                    new() { ID = "Message A", Text = "Mensagem A" },
                    new() { ID = "Message B", Text = "Mensagem B" },
                    new() { ID = "Message C", Text = "Mensagem C" },
                ]
            };
            var sut = new StringLocalizer(
                culture,
                resourceName,
                resourcePath,
                region,
                (c, rn, np) => NullStringLocalizer.Instance
            );

            // act
            var actual = sut
                .GetAllStrings(includeParentCultures: false)
                .Select(localizedString => localizedString.Name);

            var expected = region
                .Messages
                .Select(message => message.ID);

            // assert
            Assert.That(actual, Is.EquivalentTo(expected));
        }

        [Test]
        public void GetAllStrings_Should_Return_All_Messages_Including_Parent_Cultures() {
            // arrange
            var culture = new CultureInfo("pt-BR");
            const string resourceName = "AssemblyName";
            const string resourcePath = "ClassName";
            var regionPtBr = new Region {
                Name = $"[{resourceName}] {resourcePath}",
                Messages = [
                    new() { ID = "Message A", Text = "Mensagem A => pt-BR" },
                    new() { ID = "Message B", Text = "Mensagem B => pt-BR" },
                    new() { ID = "Message C", Text = "Mensagem C => pt-BR" },
                ]
            };
            var regionPtPt = new Region {
                Name = $"[{resourceName}] {resourcePath}",
                Messages = [
                    new() { ID = "Message A", Text = "Mensagem A => pt-PT" },
                    new() { ID = "Message B", Text = "Mensagem B => pt-PT" },
                    new() { ID = "Message C", Text = "Mensagem C => pt-PT" },
                ]
            };

            static IStringLocalizer factory(CultureInfo culture, string resourceName, string resourcePath) {
                var innerRegion = new Region {
                    Name = $"[{resourceName}] {resourcePath}",
                    Messages = [
                        new() { ID = "Message A", Text = $"Mensagem A => {culture.Name}" },
                        new() { ID = "Message B", Text = $"Mensagem B => {culture.Name}" },
                        new() { ID = "Message C", Text = $"Mensagem C => {culture.Name}" },
                    ]
                };

                return new StringLocalizer(culture, resourceName, resourcePath, innerRegion, factory);
            };
            var sut = new StringLocalizer(
                culture,
                resourceName,
                resourcePath,
                regionPtBr,
                factory
            );

            // act
            var actual = sut
                .GetAllStrings(includeParentCultures: true)
                .Select(localizedString => localizedString.Name);

            var expected = 
                regionPtBr
                    .Messages
                    .Select(message => message.ID)
                    .Concat(regionPtPt.Messages.Select(message => message.ID));

            // assert
            Assert.That(actual, Is.EquivalentTo(expected));
        }
    }
}
