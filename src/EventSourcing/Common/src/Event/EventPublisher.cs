using System.Threading;
using System.Threading.Tasks;
using Nameless.PubSub;

namespace Nameless.EventSourcing.Event {
    public class EventPublisher : IEventPublisher {
        #region Private Read-Only Fields

        private readonly IPublisher _publisher;
        private readonly IEventSerializer _serializer;

        #endregion

        #region Public Constructors

        public EventPublisher (IPublisher publisher, IEventSerializer serializer) {
            Prevent.ParameterNull (publisher, nameof (publisher));
            Prevent.ParameterNull (serializer, nameof (serializer));

            _publisher = publisher;
            _serializer = serializer;
        }

        #endregion

        #region IEventPublisher Members

        /// <inheritdoc />
        public Task PublishAsync<TEvent> (TEvent evt, CancellationToken token = default) where TEvent : IEvent {
            Prevent.ParameterNull (evt, nameof (evt));

            var message = new Message {
                Type = evt.GetType ().FullName,
                Payload = _serializer.Serialize (evt)
            };

            return _publisher.PublishAsync (evt.GetType ().FullName, message, token);
        }

        #endregion
    }
}