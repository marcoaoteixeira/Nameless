using MimeKit;
using MimeKit.Text;
using Nameless.Messenger.Email.Impl;
using Nameless.Test.Utils;

namespace Nameless.Messenger.Email {
    [Category(Categories.RUNS_ON_DEV_MACHINE)]
    public class SmtpClientDeliveryHandlerTests {
        [Test]
        public async Task Send_Email_Using_SmtpClientDeliveryHandler() {
            // arrange
            var smtpClientFactory = new SmtpClientFactory(new MessengerOptions {
                DeliveryMode = DeliveryMode.Network,
                Host = "localhost",
                Port = 2525,
            });

            var sut = new SmtpClientDeliveryHandler(smtpClientFactory);
            var message = new MimeMessage {
                Sender = MailboxAddress.Parse("sender@mail.com"),
                Body = new TextPart(TextFormat.Plain) {
                    Text = "This is a test email"
                }
            };
            message.To.Add(MailboxAddress.Parse("recipient@mail.com"));

            // act
            var result = await sut.HandleAsync(message, CancellationToken.None);

            // assert
            Assert.That(result, Is.Not.Empty);
        }
    }
}
