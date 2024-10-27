using System.Globalization;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging.Abstractions;
using Nameless.Localization.Json.Objects;

namespace Nameless.Localization.Json;

public class StringLocalizerTests {
    private static StringLocalizer CreateSut(CultureInfo culture, string resourceName, string resourcePath, Region region) =>
        new(culture,
            resourceName,
            resourcePath,
            region,
            (_, _, _) => NullStringLocalizer.Instance,
            NullLogger<StringLocalizer>.Instance);

    [Test]
    public void Indexer_Should_Return_LocalizedString() {
        // arrange
        const string messageID = "This is a test";
        const string expected = "Isso é um teste";
        var culture = new CultureInfo("pt-BR");
        const string resourceName = "AssemblyName";
        const string resourcePath = "ClassName";
        var region = new Region($"[{resourceName}] {resourcePath}", [new Message(messageID, expected)]);

        var sut = CreateSut(culture, resourceName, resourcePath, region);

        // act
        var actual = sut[messageID];

        // assert
        Assert.That(actual.Value, Is.EqualTo(expected));
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
        var region = new Region($"[{resourceName}] {resourcePath}", [new Message(messageID, expected)]);
        var sut = CreateSut(culture, resourceName, resourcePath, region);

        // act
        var actual = sut[messageID, args];

        // assert
        Assert.That(actual.Value, Is.EqualTo(expected));
    }

    [Test]
    public void GetAllStrings_Should_Return_All_Messages() {
        // arrange
        var culture = new CultureInfo("pt-BR");
        const string resourceName = "AssemblyName";
        const string resourcePath = "ClassName";
        var region = new Region($"[{resourceName}] {resourcePath}", [
            new Message("Message A", "Message A"),
            new Message("Message A", "Message A"),
            new Message("Message A", "Message A")
        ]);
        var sut = CreateSut(culture, resourceName, resourcePath, region);

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
        var regionPtBr = new Region($"[{resourceName}] {resourcePath}", [
            new Message("Message A", "Mensagem A => pt-BR"),
            new Message("Message B", "Mensagem B => pt-BR"),
            new Message("Message C", "Mensagem C => pt-BR"),
        ]);
        var regionPtPt = new Region($"[{resourceName}] {resourcePath}", [
            new Message("Message A", "Mensagem A => pt-PT"),
            new Message("Message B", "Mensagem B => pt-PT"),
            new Message("Message C", "Mensagem C => pt-PT"),
        ]);

        static IStringLocalizer factory(CultureInfo culture, string resourceName, string resourcePath) {
            var innerRegion = new Region($"[{resourceName}] {resourcePath}", [
                new Message("Message A", $"Mensagem A => {culture.Name}"),
                new Message("Message B", $"Mensagem B => {culture.Name}"),
                new Message("Message C", $"Mensagem C => {culture.Name}"),
            ]);

            return new StringLocalizer(culture,
                                       resourceName,
                                       resourcePath,
                                       innerRegion,
                                       factory,
                                       NullLogger<StringLocalizer>.Instance);
        }

        var sut = new StringLocalizer(
            culture,
            resourceName,
            resourcePath,
            regionPtBr,
            factory,
            NullLogger<StringLocalizer>.Instance
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