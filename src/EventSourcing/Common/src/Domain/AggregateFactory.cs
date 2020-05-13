using System;
using System.Threading;
using System.Threading.Tasks;

namespace Nameless.EventSourcing.Domain {
    public sealed class AggregateFactory : IAggregateFactory {
        #region IAggregateFactory Members

        /// <inheritdoc />
        public Task<TAggregate> CreateAsync<TAggregate> (CancellationToken token = default, params object[] args) where TAggregate : AggregateRoot {
            var aggregate = (TAggregate) Activator.CreateInstance (typeof (TAggregate), args: args);

            return Task.FromResult (aggregate);
        }

        #endregion
    }
}