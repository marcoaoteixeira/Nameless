using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dasync.Collections;
using Nameless.EventSourcing.Stores.NHibernate;
using Nameless.EventSourcing.Stores.NHibernate.Models;
using Nameless.Serialization;
using NHibernate;
using NHibernate.Criterion;

namespace Nameless.EventSourcing.Event {
    public sealed class EventStore : StoreBase, IEventStore {

        #region Public Constructors

        public EventStore (ISession session, ISerializer serializer) : base (session, serializer) { }

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
            var criteria = Session.CreateCriteria<EventEntity> ();

            criteria
                .Add (Restrictions.Eq (nameof (EventEntity.AggregateID), aggregateID.ToString ()));

            if (fromVersion.HasValue) {
                criteria
                    .Add (Restrictions.Ge (nameof (EventEntity.Version), fromVersion.Value));
            }

            return new AsyncEnumerable<IEvent> (async enumerator => {
                foreach (var entity in criteria.List<EventEntity> ()) {
                    var evt = ToEvent (entity);
                    await enumerator.ReturnAsync (evt);
                }
            });
        }

        public Task<IEvent> GetLastEventAsync (Guid aggregateID, CancellationToken token = default) {
            var criteria = Session.CreateCriteria<EventEntity> ();

            criteria
                .Add (Restrictions.Eq (nameof (EventEntity.AggregateID), aggregateID.ToString ()))
                .AddOrder (Order.Desc (nameof (EventEntity.Version)))
                .SetMaxResults (1);

            token.ThrowIfCancellationRequested ();

            var entity = criteria.UniqueResult<EventEntity> ();
            var evt = entity != null ? ToEvent (entity) : null;

            return Task.FromResult (evt);
        }

        public Task SaveAsync (IEnumerable<IEvent> events, CancellationToken token = default) {
            using (var transaction = Session.BeginTransaction ()) {
                foreach (var evt in events) {
                    token.ThrowIfCancellationRequested ();

                    var entity = ToEventEntity (evt);
                    Session.Save (entity);
                }
                Session.Flush ();
                transaction.Commit ();
            }
            return Task.CompletedTask;
        }

        #endregion
    }
}