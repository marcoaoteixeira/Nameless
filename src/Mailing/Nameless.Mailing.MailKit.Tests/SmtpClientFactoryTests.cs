﻿using System.Net.Sockets;
using MailKit.Net.Smtp;
using Nameless.Mailing.MailKit.Options;
using Nameless.Mockers;

namespace Nameless.Mailing.MailKit;

[Category(Categories.RUNS_ON_DEV_MACHINE)]
public class SmtpClientFactoryTests {
    [Test]
    public async Task Create_New_Smtp_Client() {
        // arrange
        var options = new OptionsMocker<MailingOptions>().WithValue(new MailingOptions {
                                                             Host = "localhost",
                                                             Port = 5025,
                                                         }).Build();
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

        } catch (SocketException) { Assert.Inconclusive("SMTP server unavailable."); }
        finally { client?.Dispose(); }
    }
}