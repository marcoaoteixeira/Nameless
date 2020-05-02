using System;
using Nameless.Serialization;

namespace Nameless.EventSourcing.Event {
    public class EventSerializer : IEventSerializer {
        #region Private Read-Only Fields

        private readonly ISerializer _serializer;

        #endregion

        #region Public Constructors

        public EventSerializer (ISerializer serializer) {
            Prevent.ParameterNull (serializer, nameof (serializer));

            _serializer = serializer;
        }

        #endregion

        #region IEventSerializer Members

        public byte[] Serialize (IEvent evt) {
            Prevent.ParameterNull (evt, nameof (evt));

            var result = _serializer.Serialize (evt);
            return result;
        }

        public IEvent Deserialize (Type eventType, byte[] payload) {
            Prevent.ParameterNull (eventType, nameof (eventType));
            Prevent.ParameterNull (payload, nameof (payload));

            if (!typeof (IEvent).IsAssignableFrom (eventType)) {
                throw new InvalidCastException ($"Event type is not assignable to {typeof (IEvent).FullName}");
            }

            var result = _serializer.Deserialize (eventType, payload);
            return (IEvent) result;
        }

        #endregion
    }
}