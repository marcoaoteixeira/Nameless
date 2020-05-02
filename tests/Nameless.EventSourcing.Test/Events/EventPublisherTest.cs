using System;
using System.Text;
using System.Threading;
using Moq;
using Nameless.EventSourcing.Event;
using Nameless.EventSourcing.Test.Fixtures;
using Nameless.PubSub;
using Xunit;

namespace Nameless.EventSourcing.Test.Events {
    public class EventPublisherTest {
        [Fact]
        public void Can_Create_Instance () {
            // arrange
            IEventPublisher eventPublisher;
            Mock<IPublisher> publisher = new Mock<IPublisher> ();
            Mock<IEventSerializer> serializer = new Mock<IEventSerializer> ();

            // act
            eventPublisher = new EventPublisher (publisher.Object, serializer.Object);

            // assert
            Assert.NotNull (eventPublisher);
        }

        [Fact]
        public async void Can_Publish_Event () {
            // arrange
            string messageText = "This is a test";
            Message currentMessage = null;
            Mock<IPublisher> publisher = new Mock<IPublisher> ();
            publisher
                .Setup (_ => _.PublishAsync (typeof (ChangeOrderDeliveryDateEvent).FullName, It.IsAny<Message> (), It.IsAny<CancellationToken> ()))
                .Callback ((string topic, Message message, CancellationToken token) => {
                    currentMessage = message;
                });
            Mock<IEventSerializer> serializer = new Mock<IEventSerializer> ();
            serializer
                .Setup (_ => _.Serialize (It.IsAny<IEvent> ()))
                .Returns (Encoding.UTF8.GetBytes (messageText));
            IEventPublisher eventPublisher = new EventPublisher (publisher.Object, serializer.Object);
            ChangeOrderDeliveryDateEvent evt = new ChangeOrderDeliveryDateEvent (Guid.NewGuid ());

            // act
            await eventPublisher.PublishAsync (evt);

            // assert
            Assert.NotNull (eventPublisher);
            Assert.NotNull (currentMessage);
            Assert.Equal (messageText, Encoding.UTF8.GetString (currentMessage.Payload as byte[]));
        }
    }
}
