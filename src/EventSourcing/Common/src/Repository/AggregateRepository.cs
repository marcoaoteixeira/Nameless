using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Nameless.EventSourcing.Domain;
using Nameless.EventSourcing.Event;
using Nameless.EventSourcing.Snapshot;
using Nameless.Services;

namespace Nameless.EventSourcing.Repository {
    public class AggregateRepository : IAggregateRepository {
        #region Private Read-Only Fields

        private readonly IAggregateFactory _aggregateFactory;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IEventPublisher _eventPublisher;
        private readonly IEventStore _eventStore;
        private readonly ISnapshotStore _snapshotStore;
        private readonly ISnapshotStrategy _snapshotStrategy;

        #endregion

        #region Public Constructors

        public AggregateRepository (IAggregateFactory aggregateFactory, IDateTimeProvider dateTimeProvider, IEventPublisher eventPublisher, IEventStore eventStore, ISnapshotStore snapshotStore, ISnapshotStrategy snapshotStrategy) {
            Prevent.ParameterNull (aggregateFactory, nameof (aggregateFactory));
            Prevent.ParameterNull (dateTimeProvider, nameof (dateTimeProvider));
            Prevent.ParameterNull (eventPublisher, nameof (eventPublisher));
            Prevent.ParameterNull (eventStore, nameof (eventStore));
            Prevent.ParameterNull (snapshotStore, nameof (snapshotStore));
            Prevent.ParameterNull (snapshotStrategy, nameof (snapshotStrategy));

            _aggregateFactory = aggregateFactory;
            _dateTimeProvider = dateTimeProvider;
            _eventPublisher = eventPublisher;
            _eventStore = eventStore;
            _snapshotStore = snapshotStore;
            _snapshotStrategy = snapshotStrategy;
        }

        #endregion

        #region Private Methods

        private async Task<TAggregate> GetFromSnapshotStoreAsync<TAggregate> (Guid aggregateID, CancellationToken token) where TAggregate : AggregateRoot {
            var isSnapshotable = _snapshotStrategy.IsSnapshottable (typeof (TAggregate));
            if (!isSnapshotable) { return null; }

            var snapshot = await _snapshotStore.GetAsync (aggregateID, token);
            if (snapshot == null) { return null; }

            var aggregate = await _aggregateFactory.CreateAsync<TAggregate> () as Snapshottable;
            aggregate.ApplySnapshot (snapshot);
            var eventEnumerator = _eventStore.GetAsync (aggregateID, fromVersion : snapshot.Version + 1).GetAsyncEnumerator (token);
            var events = new List<IEvent> ();
            while (await eventEnumerator.MoveNextAsync ()) {
                events.Add (eventEnumerator.Current);
            }
            aggregate.LoadFromHistory (events);

            return aggregate as TAggregate;
        }

        private async Task<TAggregate> GetFromEventStoreAsync<TAggregate> (Guid aggregateID, CancellationToken token) where TAggregate : AggregateRoot {
            var eventEnumerator = _eventStore.GetAsync (aggregateID, fromVersion : 0).GetAsyncEnumerator (token);
            var events = new List<IEvent> ();
            while (await eventEnumerator.MoveNextAsync ()) {
                events.Add (eventEnumerator.Current);
            }
            var aggregate = await _aggregateFactory.CreateAsync<TAggregate> ();

            aggregate.LoadFromHistory (events);

            return aggregate;
        }

        #endregion

        #region IRepository<TAggregate> Members

        public Task<TAggregate> GetAsync<TAggregate> (Guid aggregateID, CancellationToken token = default) where TAggregate : AggregateRoot {
            return GetFromSnapshotStoreAsync<TAggregate> (aggregateID, token) ?? GetFromEventStoreAsync<TAggregate> (aggregateID, token);
        }

        public async Task SaveAsync (AggregateRoot aggregate, CancellationToken token = default) {
            Prevent.ParameterNull (aggregate, nameof (aggregate));

            if (aggregate.HasUncommittedEvents ()) {
                var expectedVersion = aggregate.LastCommittedVersion;
                var lastEvent = await _eventStore.GetLastEventAsync (aggregate.AggregateID, token);
                if (lastEvent != null && expectedVersion == (int) StreamState.NoStream) {
                    throw new InvalidOperationException ($"Aggregate ({lastEvent.AggregateID}) can't be created as it already exists with version {(lastEvent.Version + 1)}");
                }
                if (lastEvent != null && (lastEvent.Version + 1) != expectedVersion) {
                    throw new ConcurrencyException ($"Aggregate {lastEvent.AggregateID} has been modified externally and has an updated state. Can't commit changes.");
                }

                var eventsToCommit = aggregate.GetUncommittedEvents ();
                eventsToCommit.Each ((evt, idx) => {
                    evt.TimeStamp = _dateTimeProvider.OffsetNow;
                    evt.Version = aggregate.CurrentVersion + idx + 1;
                });

                await _eventStore.SaveAsync (eventsToCommit, token);
                foreach (var evt in eventsToCommit) {
                    await _eventPublisher.PublishAsync (evt, token);
                }

                var isSnapshottable = _snapshotStrategy.IsSnapshottable (aggregate.GetType ());
                var shouldMakeSnapshot = _snapshotStrategy.ShouldMakeSnapshot (aggregate);
                if (isSnapshottable && shouldMakeSnapshot) {
                    var snapshottable = (Snapshottable) aggregate;
                    var snapshot = snapshottable.TakeSnapshot ();
                    await _snapshotStore.SaveAsync (snapshot, token);
                }

                aggregate.MarkAsCommitted ();
            }
        }

        #endregion
    }
}