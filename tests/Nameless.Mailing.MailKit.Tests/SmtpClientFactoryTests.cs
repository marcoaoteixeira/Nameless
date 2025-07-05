using MailKit.Net.Smtp;
using Nameless.Testing.Tools.Attributes;
using Nameless.Testing.Tools.Mockers;

namespace Nameless.Mailing.MailKit;

[IntegrationTest]
[Collection(nameof(Smtp4DevContainerCollection))]
public class SmtpClientFactoryTests {
    [Fact]
    public async Task Create_New_Smtp_Client() {
        // arrange
        var options = new OptionsMocker<MailingOptions>()
                     .WithValue(new MailingOptions {
                         Host = "localhost",
                         Port = Smtp4DevContainer.SMTP_PORT
                     })
                     .Build();
        var sut = new SmtpClientFactory(options);

        // act
        var client = await sut.CreateAsync(CancellationToken.None);

        // assert
        Assert.IsType<SmtpClient>(client);
    }
}