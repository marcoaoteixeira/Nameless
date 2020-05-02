using System;
using System.Data;
using Nameless.Data;
using Nameless.EventSourcing.Snapshot;

namespace Nameless.EventSourcing.Data {
    public class SnapshotEntity {
        #region Public Virtual Properties

        public virtual Guid AggregateID { get; set; }
        public virtual int Version { get; set; }
        public virtual string Type { get; set; }
        public virtual byte[] Payload { get; set; }

        #endregion

        #region Public Static Methods

        public static ISnapshot Map (IDataRecord record, Func<SnapshotEntity, ISnapshot> parser) {
            if (record == null) { return null; }
            var result = new SnapshotEntity {
                AggregateID = record.GetGuidOrDefault (nameof (AggregateID)).GetValueOrDefault (),
                Version = record.GetInt32OrDefault (nameof (Version)).GetValueOrDefault (),
                Type = record.GetStringOrDefault (nameof (Type)),
                Payload = record.GetBlobOrDefault (nameof (Payload))
            };
            return parser (result);
        }

        #endregion

        #region Public Virtual Methods

        public virtual bool Equals (SnapshotEntity obj) => obj != null && obj.AggregateID == AggregateID;

        #endregion

        #region Public Override Methods

        public override bool Equals (object obj) => Equals (obj as SnapshotEntity);

        public override int GetHashCode () {
            var hash = 13;
            unchecked {
                hash += AggregateID.GetHashCode () * 7;
            }
            return hash;
        }

        #endregion
    }
}