using System;
using System.Text;
using Nameless.EventSourcing.Event;
using Nameless.EventSourcing.Test.Fixtures;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Nameless.EventSourcing.Test.Events {
    public class EventSerializerTest {
        [Fact]
        public void Can_Create_Instance () {
            // arrange
            IEventSerializer serializer;

            // act
            serializer = new EventSerializer ();

            // assert
            Assert.NotNull (serializer);
        }

        [Fact]
        public void Can_Serialize_Event () {
            // arrange
            var serializer = new EventSerializer ();
            var evt = new ChangeOrderDeliveryDateEvent (Guid.NewGuid ());

            // act
            var serializedEvt = serializer.Serialize (evt);

            // assert
            Assert.NotNull (serializedEvt);
        }

        [Fact]
        public void Can_Serialize_Event_Correct () {
            // arrange
            var serializer = new EventSerializer ();
            var aggregateID = Guid.NewGuid ();
            var evt = new ChangeOrderDeliveryDateEvent (aggregateID);

            // act
            var serializedEvt = serializer.Serialize (evt);

            var jObject = (JObject)JsonConvert.DeserializeObject (Encoding.UTF8.GetString (serializedEvt));

            // assert
            Assert.NotNull (serializedEvt);
            Assert.NotNull (jObject[nameof (IEvent.AggregateID)]);
            Assert.Equal (aggregateID.ToString (), jObject[nameof (IEvent.AggregateID)]);
        }

        [Fact]
        public void Can_Deserialize_Event () {
            // arrange
            var serializer = new EventSerializer ();
            var aggregateID = Guid.NewGuid ();
            var payload = Encoding.UTF8.GetBytes ($"{{ AggregateID: '{aggregateID}' }}");

            // act
            var deserializedEvt = serializer.Deserialize (typeof (ChangeOrderDeliveryDateEvent), payload);

            // assert
            Assert.NotNull (deserializedEvt);
            Assert.IsType<ChangeOrderDeliveryDateEvent> (deserializedEvt);
        }

        [Fact]
        public void Can_Deserialize_Event_Correct () {
            // arrange
            var serializer = new EventSerializer ();
            var aggregateID = Guid.NewGuid ();
            var payload = Encoding.UTF8.GetBytes ($"{{ AggregateID: '{aggregateID}' }}");

            // act
            var deserializedEvt = serializer.Deserialize (typeof (ChangeOrderDeliveryDateEvent), payload);

            var jObject = (JObject)JsonConvert.DeserializeObject (Encoding.UTF8.GetString (payload));

            // assert
            Assert.NotNull (deserializedEvt);
            Assert.IsType<ChangeOrderDeliveryDateEvent> (deserializedEvt);
            Assert.NotNull (jObject[nameof (IEvent.AggregateID)]);
            Assert.Equal (aggregateID.ToString (), jObject[nameof (IEvent.AggregateID)]);
        }
    }
}
