using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;
using Nameless.Data;
using Nameless.EventSourcing.Data;
using Nameless.FileProvider.Common;

namespace Nameless.EventSourcing.Event {
    public class EventStore : IEventStore {
        #region Private Read-Only Fields

        private readonly IDatabase _database;
        private readonly IFileProvider _fileProvider;
        private readonly IEventSerializer _eventSerializer;

        #endregion

        #region Private Methods

        private IEvent ToEvent (EventEntity entity) {
            if (entity.Payload == null || string.IsNullOrWhiteSpace (entity.Type)) { return null; }
            var evt = _eventSerializer.Deserialize (Type.GetType (entity.Type), entity.Payload);
            return evt;
        }

        private EventEntity ToEventEntity (IEvent evt) {
            return new EventEntity {
                EventID = evt.EventID,
                AggregateID = evt.AggregateID,
                Version = evt.Version,
                TimeStamp = evt.TimeStamp,
                Type = evt.GetType ().FullName,
                Payload = _eventSerializer.Serialize (evt)
            };
        }

        #endregion

        #region Public Constructors

        public EventStore (IDatabase database, IFileProvider fileProvider, IEventSerializer eventSerializer) {
            Prevent.ParameterNull (database, nameof (database));
            Prevent.ParameterNull (fileProvider, nameof (fileProvider));
            Prevent.ParameterNull (eventSerializer, nameof (eventSerializer));

            _database = database;
            _fileProvider = fileProvider;
            _eventSerializer = eventSerializer;
        }

        #endregion

        #region IEventStore Members

        public IAsyncEnumerable<IEvent> GetAsync (Guid aggregateID, int? fromVersion) {
            var statement = _fileProvider.GetFileInfo ("Resources/EventSourcing/SQLs/Event_Select_FromVersion_Statement.sql").GetText ();

            var result = _database.ExecuteReaderAsync (
                commandText: statement,
                mapper: record => EventEntity.Map (record, ToEvent),
                parameters : new [] {
                    Parameter.CreateInputParameter (nameof (EventEntity.AggregateID), aggregateID, DbType.Guid),
                    Parameter.CreateInputParameter (nameof (EventEntity.Version), fromVersion.HasValue ? (object) fromVersion.Value : null, DbType.Int32)
                }
            );

            return result;
        }

        public async Task SaveAsync (IEnumerable<IEvent> events, CancellationToken token = default) {
            var statement = _fileProvider.GetFileInfo ("Data/SQLs/Event_Insert_Statement").GetText ();

            using (var transaction = _database.StartTransaction ()) {
                foreach (var evt in events) {
                    token.ThrowIfCancellationRequested ();

                    var entity = ToEventEntity (evt);
                    await _database.ExecuteNonQueryAsync (
                        commandText: statement,
                        token: token,
                        parameters: new [] {
                            Parameter.CreateInputParameter (nameof (EventEntity.EventID), entity.EventID, DbType.Guid),
                            Parameter.CreateInputParameter (nameof (EventEntity.AggregateID), entity.AggregateID, DbType.Guid),
                            Parameter.CreateInputParameter (nameof (EventEntity.Version), entity.Version, DbType.Int32),
                            Parameter.CreateInputParameter (nameof (EventEntity.TimeStamp), entity.TimeStamp, DbType.DateTimeOffset),
                            Parameter.CreateInputParameter (nameof (EventEntity.Type), entity.Type, DbType.String),
                            Parameter.CreateInputParameter (nameof (EventEntity.Payload), entity.Payload, DbType.Binary)
                        });
                }
                transaction.Commit ();
            }
        }

        public Task<IEvent> GetLastEventAsync (Guid aggregateID, CancellationToken token = default) {
            var statement = _fileProvider.GetFileInfo ("Data/SQLs/Event_Select_Last_Statement").GetText ();

            return _database.ExecuteReaderSingleAsync (
                commandText: statement,
                mapper: record => EventEntity.Map (record, ToEvent),
                token : token,
                parameters : new [] {
                    Parameter.CreateInputParameter (nameof (EventEntity.AggregateID), aggregateID, DbType.Guid)
                }
            );
        }

        #endregion
    }
}