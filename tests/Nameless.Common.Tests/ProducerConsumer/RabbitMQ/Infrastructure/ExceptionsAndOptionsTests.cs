using System.Security.Authentication;
using Nameless.ProducerConsumer.RabbitMQ.Options;

namespace Nameless.ProducerConsumer.RabbitMQ.Infrastructure;

[UnitTest]
public class ExceptionsAndOptionsTests {
    [Fact]
    public void MissingQueueConfigurationException_ExposesQueueNameAndMessage() {
        // act
        var sut = new MissingQueueConfigurationException("q1");

        // assert
        Assert.Multiple(
            () => Assert.Equal("q1", sut.QueueName),
            () => Assert.Contains("q1", sut.Message)
        );
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void NoRetryException_ExposesRequeue(bool requeue) {
        // act
        var sut = new NoRetryException("stop", requeue);

        // assert
        Assert.Multiple(
            () => Assert.Equal(requeue, sut.Requeue),
            () => Assert.Equal("stop", sut.Message)
        );
    }

    [Fact]
    public void NoRetryException_DefaultsToNoRequeue() {
        // act & assert
        Assert.False(new NoRetryException("stop").Requeue);
    }

    [Theory]
    [InlineData(true, "host", SslProtocols.Tls12, true)]
    [InlineData(false, "host", SslProtocols.Tls12, false)]
    [InlineData(true, " ", SslProtocols.Tls12, false)]
    [InlineData(true, "host", SslProtocols.None, false)]
    public void SslOptions_IsAvailable_RequiresEnabledServerNameAndProtocol(bool enabled, string serverName, SslProtocols protocol, bool expected) {
        // arrange
        var sut = new SslOptions { Enabled = enabled, ServerName = serverName, Protocol = protocol };

        // act & assert
        Assert.Equal(expected, sut.IsAvailable);
    }

    [Theory]
    [InlineData("a.pfx", "pwd", true)]
    [InlineData(null, "pwd", false)]
    [InlineData("a.pfx", null, false)]
    [InlineData(" ", " ", false)]
    public void CertificateOptions_IsAvailable_RequiresPathAndPassword(string? path, string? password, bool expected) {
        // arrange
        var sut = new CertificateOptions { CertPath = path, CertPassword = password };

        // act & assert
        Assert.Equal(expected, sut.IsAvailable);
    }
}
