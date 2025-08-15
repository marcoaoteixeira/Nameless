using MailKit.Net.Smtp;
using Nameless.Testing.Tools;
using Nameless.Testing.Tools.Attributes;

namespace Nameless.Mailing.MailKit;

[IntegrationTest]
[Collection(nameof(Smtp4DevContainerCollection))]
public class SmtpClientFactoryTests {
    [Fact]
    public async Task Create_New_Smtp_Client() {
        // arrange
        var options = OptionsHelper.Create<MailingOptions>(opts => {
            opts.Host = "localhost";
            opts.Port = Smtp4DevContainer.SMTP_PORT;
        });
        var sut = new SmtpClientFactory(options);

        // act
        var client = await sut.CreateAsync(CancellationToken.None);

        // assert
        Assert.IsType<SmtpClient>(client);
    }
}