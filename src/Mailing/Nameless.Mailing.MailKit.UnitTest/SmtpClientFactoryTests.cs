using MailKit.Net.Smtp;
using Nameless.Mailing.MailKit.Impl;
using Nameless.Mailing.MailKit.Options;

namespace Nameless.Mailing.MailKit;

[Category(Categories.RUNS_ON_DEV_MACHINE)]
public class SmtpClientFactoryTests {
    [Test]
    public async Task Create_New_Smtp_Client() {
        // arrange
        var smtpClientFactory = new SmtpClientFactory(new MailServerOptions {
            Host = "localhost",
            Port = 5025,
        });

        // act
        using var result = await smtpClientFactory.CreateAsync(CancellationToken.None);

        // assert
        Assert.Multiple(() => {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<SmtpClient>());
        });
    }
}