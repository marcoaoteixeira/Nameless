using MailKit.Net.Smtp;
using Nameless.Mailing.MailKit.Impl;
using Nameless.Mailing.MailKit.Options;

namespace Nameless.Mailing.MailKit;

[Category(Categories.RUNS_ON_DEV_MACHINE)]
public class SmtpClientFactoryTests {
    [Test]
    public async Task Create_New_Smtp_Client() {
        // arrange
        var options = Microsoft.Extensions.Options.Options.Create(new MailServerOptions { Host = "localhost", Port = 5025, });
        var smtpClientFactory = new SmtpClientFactory(options);

        // act
        using var result = await smtpClientFactory.CreateAsync(CancellationToken.None);

        // assert
        Assert.Multiple(() => {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<SmtpClient>());
        });
    }
}