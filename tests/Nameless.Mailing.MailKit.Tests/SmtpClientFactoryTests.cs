using System.Net.Sockets;
using MailKit.Net.Smtp;
using Nameless.Testing.Tools;
using Nameless.Testing.Tools.Mockers;

namespace Nameless.Mailing.MailKit;

[Category(Categories.RunsOnDevMachine)]
public class SmtpClientFactoryTests {
    [Fact]
    public async Task Create_New_Smtp_Client() {
        // arrange
        var options = new OptionsMocker<MailingOptions>()
                     .WithValue(new MailingOptions { Host = "localhost", Port = 5025 }).Build();
        var smtpClientFactory = new SmtpClientFactory(options);

        // act
        ISmtpClient client = null;
        try {
            client = await smtpClientFactory.CreateAsync(CancellationToken.None);

            // assert
            Assert.Multiple(() => {
                Assert.That(client, Is.Not.Null);
                Assert.That(client, Is.InstanceOf<SmtpClient>());
            });
        }
        catch (SocketException) { Assert.Inconclusive("SMTP server unavailable."); }
        finally { client?.Dispose(); }
    }
}