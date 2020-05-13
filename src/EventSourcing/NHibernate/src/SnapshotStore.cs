using System;
using System.Threading;
using System.Threading.Tasks;
using Nameless.EventSourcing.Stores.NHibernate;
using Nameless.EventSourcing.Stores.NHibernate.Models;
using Nameless.Persistence;
using Nameless.Serialization;

namespace Nameless.EventSourcing.Snapshot {
    public sealed class SnapshotStore : StoreBase, ISnapshotStore {

        #region Public Constructors

        public SnapshotStore (IRepository repository, ISerializer serializer) : base (repository, serializer) { }

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
            return Repository
                .FindOneAsync<SnapshotEntity> (_ => _.AggregateID == aggregateID, token)
                .ContinueWith (continuation => {
                    ISnapshot result = null;
                    if (continuation.CanContinue ()) {
                        result = continuation.Result != null ? ToSnapshot (continuation.Result) : null;
                    }
                    return result;
                }, TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        public Task SaveAsync (ISnapshot snapshot, CancellationToken token = default) {
            var entity = ToSnapshotEntity (snapshot);

            token.ThrowIfCancellationRequested ();

            return Repository.SaveAsync (new [] { entity }, token : token);
        }

        #endregion
    }
}