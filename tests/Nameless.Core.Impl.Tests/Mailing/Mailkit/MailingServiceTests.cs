using MailKit;
using MailKit.Net.Smtp;
using MimeKit;
using Moq;
using Nameless.Mailing;
using Nameless.Mailing.Mailkit;
using Nameless.Testing.Tools.Attributes;
using Nameless.Testing.Tools.Mockers.Logging;

namespace Nameless.Mailing.Mailkit;

public class MailingServiceTests {
    private static MailingService CreateSut(ISmtpClientFactory factory) {
        var logger = new LoggerMocker<MailingService>()
            .WithAnyLogLevel()
            .Build();

        return new MailingService(factory, logger);
    }

    private static Message CreateValidMessage() {
        return new Message(
            subject: "Test Subject",
            from: ["sender@example.com"],
            to: ["recipient@example.com"],
            content: "Hello, world!"
        );
    }

    [Fact]
    [UnitTest]
    public async Task DeliverAsync_ValidMessage_SendsViaSmtpClient() {
        // arrange
        var smtpClientMock = new Mock<ISmtpClient>(MockBehavior.Loose);

        smtpClientMock
            .Setup(c => c.SendAsync(
                It.IsAny<MimeMessage>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<ITransferProgress>()
            ))
            .ReturnsAsync(string.Empty);

        smtpClientMock
            .Setup(c => c.DisconnectAsync(It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var factoryMock = new Mock<ISmtpClientFactory>(MockBehavior.Strict);

        factoryMock
            .Setup(f => f.CreateAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(smtpClientMock.Object);

        var sut = CreateSut(factoryMock.Object);
        var message = CreateValidMessage();

        // act
        await sut.DeliverAsync(message, CancellationToken.None);

        // assert
        smtpClientMock.Verify(
            c => c.SendAsync(
                It.IsAny<MimeMessage>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<ITransferProgress>()
            ),
            Times.Once
        );
    }

    [Fact]
    [UnitTest]
    public async Task DeliverAsync_SmtpClientThrows_PropagatesException() {
        // arrange
        var smtpClientMock = new Mock<ISmtpClient>(MockBehavior.Loose);

        smtpClientMock
            .Setup(c => c.SendAsync(
                It.IsAny<MimeMessage>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<ITransferProgress>()
            ))
            .ThrowsAsync(new InvalidOperationException("SMTP connection failed"));

        var factoryMock = new Mock<ISmtpClientFactory>(MockBehavior.Strict);

        factoryMock
            .Setup(f => f.CreateAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(smtpClientMock.Object);

        var sut = CreateSut(factoryMock.Object);
        var message = CreateValidMessage();

        // act & assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => sut.DeliverAsync(message, CancellationToken.None)
        );
    }
}
