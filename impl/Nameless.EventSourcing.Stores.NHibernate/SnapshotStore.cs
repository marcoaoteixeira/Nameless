using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Nameless.EventSourcing.Stores.NHibernate;
using Nameless.EventSourcing.Stores.NHibernate.Models;
using Nameless.Serialization;
using NHibernate;
using NHibernate.Criterion;

namespace Nameless.EventSourcing.Snapshot {
    public sealed class SnapshotStore : StoreBase, ISnapshotStore {

        #region Public Constructors

        public SnapshotStore (ISession session, ISerializer serializer) : base (session, serializer) { }

        #endregion

        #region Private Methods

        private ISnapshot ToSnapshot (SnapshotEntity entity) {
            if (entity.Payload == null || string.IsNullOrWhiteSpace (entity.Type)) { return null; }
            var result = (ISnapshot) Serializer.Deserialize (Type.GetType (entity.Type), entity.Payload);
            return result;
        }

        private SnapshotEntity ToSnapshotEntity (ISnapshot snapshot) {
            return new SnapshotEntity {
                AggregateID = snapshot.AggregateID,
                Version = snapshot.Version,
                Type = snapshot.GetType ().FullName,
                Payload = Serializer.Serialize (snapshot)
            };
        }

        #endregion

        #region ISnapshotStore Members

        public Task<ISnapshot> GetAsync (Guid aggregateID, CancellationToken token = default) {
            var criteria = Session.CreateCriteria<SnapshotEntity> ();

            criteria
                .Add (Restrictions.Eq (nameof (SnapshotEntity.AggregateID), aggregateID.ToString ()));

            token.ThrowIfCancellationRequested ();

            var entity = criteria.List<SnapshotEntity> ().SingleOrDefault ();
            var snapshot = entity != null ? ToSnapshot (entity) : null;

            return Task.FromResult (snapshot);
        }

        public Task SaveAsync (ISnapshot snapshot, CancellationToken token = default) {
            var entity = ToSnapshotEntity (snapshot);

            token.ThrowIfCancellationRequested ();

            return Session.SaveAsync (entity, token);
        }

        #endregion
    }
}