using System;
using Nameless.EventSourcing.Event;

namespace Nameless.EventSourcing.Test.Fixtures {
    public class OrderCreatedEvent : EventBase {
        public OrderCreatedEvent (Guid aggregateID) : base (aggregateID) {
            
        }
    }
}
