using System;
using Nameless.EventSourcing.Event;

namespace Nameless.EventSourcing.Test.Fixtures {
    public class ChangeOrderDeliveryDateEvent : EventBase {
        public ChangeOrderDeliveryDateEvent (Guid aggregateID) : base (aggregateID) {
        }
    }
}