using System;
using System.Data;
using Nameless.EventSourcing.Snapshot;

namespace Nameless.EventSourcing.Stores.NHibernate.Models {
    public class SnapshotEntity {
        #region Public Virtual Properties

        public virtual Guid AggregateID { get; set; }
        public virtual int Version { get; set; }
        public virtual string Type { get; set; }
        public virtual byte[] Payload { get; set; }

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