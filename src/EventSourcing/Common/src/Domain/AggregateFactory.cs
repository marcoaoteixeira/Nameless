using System;

namespace Nameless.EventSourcing.Domain {
    public sealed class AggregateFactory : IAggregateFactory {
        #region IAggregateFactory Members

        /// <inheritdoc />
        public TAggregate Create<TAggregate> (params object[] args) where TAggregate : AggregateRoot {
            var aggregate = (TAggregate) Activator.CreateInstance (typeof (TAggregate), args);

            return aggregate;
        }

        #endregion
    }
}