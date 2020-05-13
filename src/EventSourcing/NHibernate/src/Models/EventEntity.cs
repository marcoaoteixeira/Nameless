using System;

namespace Nameless.EventSourcing.Stores.NHibernate.Models {
    public class EventEntity {
        #region Public Virtual Properties

        public virtual Guid EventID { get; set; }
        public virtual Guid AggregateID { get; set; }
        public virtual int Version { get; set; }
        public virtual DateTimeOffset TimeStamp { get; set; }
        public virtual string Type { get; set; }
        public virtual byte[] Payload { get; set; }

        #endregion

        #region Public Virtual Methods

        public virtual bool Equals (EventEntity obj) {
            return obj != null &&
                obj.EventID == EventID &&
                obj.AggregateID == AggregateID;
        }

        #endregion

        #region Public Override Methods

        public override bool Equals (object obj) {
            return Equals (obj as EventEntity);
        }

        public override int GetHashCode () {
            var hash = 13;
            unchecked {
                hash += EventID.GetHashCode () * 7;
                hash += AggregateID.GetHashCode () * 7;
            }
            return hash;
        }

        #endregion
    }
}