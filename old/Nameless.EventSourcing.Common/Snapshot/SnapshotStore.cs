using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;
using Nameless.Data;
using Nameless.EventSourcing.Data;
using Nameless.FileProvider.Common;
using Nameless.Serialization;

namespace Nameless.EventSourcing.Snapshot {
    public sealed class SnapshotStore : ISnapshotStore {
        #region Private Read-Only Fields

        private readonly IDatabase _database;
        private readonly IFileProvider _fileProvider;
        private readonly ISerializer _serializer;

        #endregion

        #region Public Constructors

        public SnapshotStore (IDatabase database, IFileProvider fileProvider, ISerializer serializer) {
            Prevent.ParameterNull (database, nameof (database));
            Prevent.ParameterNull (fileProvider, nameof (fileProvider));
            Prevent.ParameterNull (serializer, nameof (serializer));

            _database = database;
            _fileProvider = fileProvider;
            _serializer = serializer;
        }

        #endregion

        #region Private Methods

        private ISnapshot ToSnapshot (SnapshotEntity entity) {
            if (entity.Payload == null || string.IsNullOrWhiteSpace (entity.Type)) { return null; }
            var result = (ISnapshot) _serializer.Deserialize (Type.GetType (entity.Type), entity.Payload);
            return result;
        }

        private SnapshotEntity ToSnapshotEntity (ISnapshot snapshot) {
            return new SnapshotEntity {
                AggregateID = snapshot.AggregateID,
                Version = snapshot.Version,
                Type = snapshot.GetType ().FullName,
                Payload = _serializer.Serialize (snapshot)
            };
        }

        #endregion

        #region ISnapshotStore Members

        public Task<ISnapshot> GetAsync (Guid aggregateID, CancellationToken token = default) {
            var statement = _fileProvider.GetFileInfo ("Resources/EventSourcing/SQLs/Snapshot_Select_Statement.sql").GetText ();

            token.ThrowIfCancellationRequested ();
            var snapshot = _database.ExecuteReaderSingleAsync (
                commandText: statement,
                mapper: record => SnapshotEntity.Map (record, ToSnapshot),
                parameters : new [] {
                    Parameter.CreateInputParameter (nameof (EventEntity.AggregateID), aggregateID, DbType.Guid)
                }
            );

            return snapshot;
        }

        public async Task SaveAsync (ISnapshot snapshot, CancellationToken token = default) {
            var statement = _fileProvider.GetFileInfo ("Resources/EventSourcing/SQLs/Snapshot_Save_Statement.sql").GetText ();

            using (var transaction = _database.StartTransaction ()) {
                token.ThrowIfCancellationRequested ();

                var entity = ToSnapshotEntity (snapshot);
                await _database.ExecuteNonQueryAsync (
                    commandText: statement,
                    token: token,
                    parameters: new [] {
                        Parameter.CreateInputParameter (nameof (SnapshotEntity.AggregateID), entity.AggregateID, DbType.Guid),
                        Parameter.CreateInputParameter (nameof (SnapshotEntity.Version), entity.Version, DbType.Int32),
                        Parameter.CreateInputParameter (nameof (SnapshotEntity.Type), entity.Type, DbType.String),
                        Parameter.CreateInputParameter (nameof (SnapshotEntity.Payload), entity.Payload, DbType.Binary)
                    });
                transaction.Commit ();
            }
        }

        #endregion

    }
}