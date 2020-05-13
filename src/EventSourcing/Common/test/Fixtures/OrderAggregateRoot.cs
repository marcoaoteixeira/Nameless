using System;
using Nameless.EventSourcing.Domain;

namespace Nameless.EventSourcing.Test.Fixtures {
    public class OrderAggregateRoot : AggregateRoot {
        #region Public Constructors

        public OrderAggregateRoot () {

        }

        public OrderAggregateRoot (Guid id) {
            ApplyEvent (new OrderCreatedEvent (id));
        }

        #endregion

        #region Public Methods

        private void OnEvent (OrderCreatedEvent evt) {
            AggregateID = evt.AggregateID;
        }

        #endregion
    }
}
