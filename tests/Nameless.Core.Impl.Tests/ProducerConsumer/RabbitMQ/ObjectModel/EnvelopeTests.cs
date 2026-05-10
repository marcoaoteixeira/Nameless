using Nameless.ProducerConsumer.RabbitMQ.ObjectModel;
using Nameless.Testing.Tools.Attributes;

namespace Nameless.ProducerConsumer.RabbitMQ.ObjectModel;

public class EnvelopeTests {
    [Fact]
    [UnitTest]
    public void Envelope_Properties_AreCorrect() {
        // arrange
        var header = new Header {
            MessageID = "msg-001",
            CorrelationID = "corr-001",
            Timestamp = 1_700_000_000L
        };

        const string payload = "hello";

        // act
        var envelope = new Envelope {
            Header = header,
            Message = payload
        };

        // assert
        Assert.Multiple(() => {
            Assert.Same(header, envelope.Header);
            Assert.Equal("msg-001", envelope.Header.MessageID);
            Assert.Equal("corr-001", envelope.Header.CorrelationID);
            Assert.Equal(1_700_000_000L, envelope.Header.Timestamp);
            Assert.Equal(payload, envelope.Message);
        });
    }

    [Fact]
    [UnitTest]
    public void Header_DefaultValues_AreNullOrZero() {
        // arrange & act
        var header = new Header();

        // assert
        Assert.Multiple(() => {
            Assert.Null(header.MessageID);
            Assert.Null(header.CorrelationID);
            Assert.Equal(0L, header.Timestamp);
        });
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

        var first = new Envelope { Header = header, Message = "payload" };
        var second = new Envelope { Header = header, Message = "payload" };

        // act & assert
        Assert.Equal(first, second);
    }
}
