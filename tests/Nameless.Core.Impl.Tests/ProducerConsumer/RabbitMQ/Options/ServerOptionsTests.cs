using Nameless.ProducerConsumer.RabbitMQ.Options;
using Nameless.Testing.Tools.Attributes;

namespace Nameless.ProducerConsumer.RabbitMQ.Options;

[UnitTest]
public class ServerOptionsTests {
    [Fact]
    public void UseCredentials_WhenUsernameAndPasswordAreSet_ReturnsTrue() {
        // arrange
        var sut = new ServerOptions {
            Username = "guest",
            Password = "guest"
        };

        // act & assert
        Assert.True(sut.UseCredentials);
    }

    [Fact]
    public void UseCredentials_WhenUsernameIsNull_ReturnsFalse() {
        // arrange
        var sut = new ServerOptions {
            Username = null,
            Password = "guest"
        };

        // act & assert
        Assert.False(sut.UseCredentials);
    }

    [Fact]
    public void UseCredentials_WhenPasswordIsNull_ReturnsFalse() {
        // arrange
        var sut = new ServerOptions {
            Username = "guest",
            Password = null
        };

        // act & assert
        Assert.False(sut.UseCredentials);
    }

    [Fact]
    public void UseCredentials_WhenUsernameIsWhitespace_ReturnsFalse() {
        // arrange
        var sut = new ServerOptions {
            Username = "   ",
            Password = "guest"
        };

        // act & assert
        Assert.False(sut.UseCredentials);
    }

    [Fact]
    public void UseCredentials_WhenBothAreNull_ReturnsFalse() {
        // arrange
        var sut = new ServerOptions();

        // act & assert
        Assert.False(sut.UseCredentials);
    }

    [Fact]
    public void Protocol_DefaultValue_IsAmqp() {
        // arrange & act
        var sut = new ServerOptions();

        // assert
        Assert.Equal("amqp", sut.Protocol);
    }

    [Fact]
    public void Hostname_DefaultValue_IsLocalhost() {
        // arrange & act
        var sut = new ServerOptions();

        // assert
        Assert.Equal("localhost", sut.Hostname);
    }

    [Fact]
    public void Port_DefaultValue_Is5672() {
        // arrange & act
        var sut = new ServerOptions();

        // assert
        Assert.Equal(5672, sut.Port);
    }

    [Fact]
    public void VirtualHost_DefaultValue_IsSlash() {
        // arrange & act
        var sut = new ServerOptions();

        // assert
        Assert.Equal("/", sut.VirtualHost);
    }

    [Fact]
    public void Ssl_DefaultValue_IsNotNull() {
        // arrange & act
        var sut = new ServerOptions();

        // assert
        Assert.NotNull(sut.Ssl);
    }

    [Fact]
    public void Certificate_DefaultValue_IsNotNull() {
        // arrange & act
        var sut = new ServerOptions();

        // assert
        Assert.NotNull(sut.Certificate);
    }
}
