namespace Nameless.ProducerConsumer.RabbitMQ.ObjectModel;

public class MessageTests {
    [Fact]
    [UnitTest]
    public void Envelope_Properties_AreCorrect() {
        // arrange
        var header = new Header {
            MessageID = "msg-001",
            CorrelationID = "corr-001",
            Timestamp = 1_700_000_000L
        };

        const string Payload = "hello";

        // act
        var message = new Message {
            Header = header,
            Content = Payload
        };

        // assert
        Assert.Multiple(
            () => Assert.Same(header, message.Header),
            () => Assert.Equal("msg-001", message.Header.MessageID),
            () => Assert.Equal("corr-001", message.Header.CorrelationID),
            () => Assert.Equal(1_700_000_000L, message.Header.Timestamp),
            () => Assert.Equal(Payload, message.Content)
        );
    }

    [Fact]
    [UnitTest]
    public void Header_DefaultValues_AreNullOrZero() {
        // arrange & act
        var header = new Header();

        // assert
        Assert.Multiple(
            () => Assert.Null(header.MessageID),
            () => Assert.Null(header.CorrelationID),
            () => Assert.Equal(0L, header.Timestamp)
        );
    }

    [Fact]
    [UnitTest]
    public void Envelope_RecordEquality_HoldsWhenPropertiesMatch() {
        // arrange
        var header = new Header {
            MessageID = "m1",
            CorrelationID = "c1",
            Timestamp = 42L
        };

        var first = new Message { Header = header, Content = "payload" };
        var second = new Message { Header = header, Content = "payload" };

        // act & assert
        Assert.Equal(first, second);
    }
}
