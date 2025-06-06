﻿using System.Globalization;
using Microsoft.Extensions.Localization;
using Nameless.Localization.Json.Mockers;
using Nameless.Localization.Json.Objects;
using Nameless.Testing.Tools.Mockers;

namespace Nameless.Localization.Json;

public class StringLocalizerTests {
    private const string BASE_NAME = "AssemblyName";
    private const string LOCATION = "ClassName";

    private static StringLocalizer CreateSut(Resource resource,
                                             Func<string, string, CultureInfo, IStringLocalizer> factory = null) {
        var emptyStringLocalizer = new StringLocalizerMocker().Build();
        var logger = new LoggerMocker<StringLocalizer>().Build();

        return new StringLocalizer(BASE_NAME,
            LOCATION,
            new CultureInfo("pt-BR"),
            resource,
            factory ?? ((_, _, _) => emptyStringLocalizer),
            logger);
    }

    private static Resource CreateResource(CultureInfo culture = null, Message[] messages = null) {
        return new Resource($"{BASE_NAME}.{LOCATION}",
            (culture ?? new CultureInfo("pt-BR")).Name,
            messages ?? [],
            true);
    }

    [Fact]
    public void WhenCallingIndexer_ThenReturnsAssociatedLocalizesMessage() {
        // arrange
        const string MessageId = "This is a test";
        const string Expected = "Isso é um teste";

        var resource = CreateResource(messages: [new Message(MessageId, Expected)]);
        var sut = CreateSut(resource);

        // act
        var actual = sut[MessageId];

        // assert
        Assert.Equal(Expected, actual.Value);
    }

    [Fact]
    public void WhenCallingIndexerWithParameters_ThenReturnsFormattedLocalizedMessage() {
        // arrange
        const string MessageId = "This is a test {0}, {1}, {2}, {3}";
        const string Expected = "Isso é um teste 1, 2, 3, 4";
        var args = new object[] { 1, 2, 3, 4 };

        var resource = CreateResource(messages: [new Message(MessageId, Expected)]);
        var sut = CreateSut(resource);

        // act
        var actual = sut[MessageId, args];

        // assert
        Assert.Equal(Expected, actual.Value);
    }

    [Fact]
    public void WhenGettingAllString_ThenReturnsAllMessages() {
        // arrange
        Message[] messages = [
            new("Message A", "Message A"),
            new("Message A", "Message A"),
            new("Message A", "Message A")
        ];
        var resource = CreateResource(messages: messages);
        var sut = CreateSut(resource);

        // act
        var actual = sut.GetAllStrings(false)
                        .Select(localizedString => localizedString.Name);

        var expected = messages.Select(message => message.Id);

        // assert
        Assert.Equivalent(expected, actual);
    }

    [Fact]
    public void WhenGettingAllStringsWithParentCulture_ThenReturnsAllMessages() {
        // arrange
        var resourcePtBr = CreateResource(messages: [
            new Message("Message A", "Mensagem A => pt-BR"),
            new Message("Message B", "Mensagem B => pt-BR"),
            new Message("Message C", "Mensagem C => pt-BR")
        ]);
        var resourcePtBrParent = CreateResource(messages: [
            new Message("Message A", "Mensagem A => pt"),
            new Message("Message B", "Mensagem B => pt"),
            new Message("Message C", "Mensagem C => pt")
        ]);
        var resourceInvariant = CreateResource(messages: [
            new Message("Message A", "Mensagem A => "),
            new Message("Message B", "Mensagem B => "),
            new Message("Message C", "Mensagem C => ")
        ]);

        var sut = CreateSut(resourcePtBr, Factory);

        // act
        var actual = sut.GetAllStrings(true)
                        .Select(localizedString => localizedString.Value)
                        .ToArray();

        var expected = resourcePtBr.Messages
                                   .Select(message => message.Text)
                                   .Concat(resourcePtBrParent.Messages.Select(message => message.Text))
                                   .Concat(resourceInvariant.Messages.Select(message => message.Text))
                                   .ToArray();

        // assert
        Assert.Equivalent(expected, actual);

        return;

        static IStringLocalizer Factory(string baseName, string location, CultureInfo culture) {
            var resource = CreateResource(culture, [
                new Message("Message A", $"Mensagem A => {culture.Name}"),
                new Message("Message B", $"Mensagem B => {culture.Name}"),
                new Message("Message C", $"Mensagem C => {culture.Name}")
            ]);

            return CreateSut(resource);
        }
    }
}