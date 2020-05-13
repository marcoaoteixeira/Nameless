using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dasync.Collections;
using Nameless.EventSourcing.Stores.NHibernate;
using Nameless.EventSourcing.Stores.NHibernate.Models;
using Nameless.Persistence;
using Nameless.Serialization;

namespace Nameless.EventSourcing.Event {
    public sealed class EventStore : StoreBase, IEventStore {

        #region Public Constructors

        public EventStore (IRepository repository, ISerializer serializer) : base (repository, serializer) { }

        #endregion

        #region Private Methods

        private IEvent ToEvent (EventEntity entity) {
            if (entity.Payload == null || string.IsNullOrWhiteSpace (entity.Type)) { return null; }
            var evt = Serializer.Deserialize (Type.GetType (entity.Type), entity.Payload);
            return (IEvent) evt;
        }

        private EventEntity ToEventEntity (IEvent evt) {
            return new EventEntity {
                EventID = evt.EventID,
                AggregateID = evt.AggregateID,
                Version = evt.Version,
                TimeStamp = evt.TimeStamp,
                Type = evt.GetType ().FullName,
                Payload = Serializer.Serialize (evt)
            };
        }

        #endregion

        #region IEventStore Members

        public IAsyncEnumerable<IEvent> GetAsync (Guid aggregateID, int? fromVersion) {
            var query = Repository.Query<EventEntity> ();

            query = query.Where (_ => _.AggregateID == aggregateID);

            if (fromVersion.HasValue) {
                query = query.Where (_ => _.Version >= fromVersion.Value);
            }

            return new AsyncEnumerable<IEvent> (async enumerator => {
                foreach (var entity in query.ToArray ()) {
                    var evt = ToEvent (entity);
                    await enumerator.ReturnAsync (evt);
                }
            });
        }

        public Task<IEvent> GetLastEventAsync (Guid aggregateID, CancellationToken token = default) {
            var query = Repository
                .Query<EventEntity> ()
                .Where (_ => _.AggregateID == aggregateID)
                .OrderBy (_ => _.Version)
                .Take (1);

            token.ThrowIfCancellationRequested ();

            var entity = query.SingleOrDefault ();
            var evt = entity != null ? ToEvent (entity) : null;

            return Task.FromResult (evt);
        }

        public Task SaveAsync (IEnumerable<IEvent> events, CancellationToken token = default) {
            var entities = new List<EventEntity> ();
            foreach (var evt in events) {
                token.ThrowIfCancellationRequested ();
                entities.Add (ToEventEntity (evt));
            }
            return Repository.SaveAsync (entities.ToArray (), token : token);
        }

        #endregion
    }
}