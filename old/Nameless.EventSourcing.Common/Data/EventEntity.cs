using System;
using System.Data;
using Nameless.Data;
using Nameless.EventSourcing.Event;

namespace Nameless.EventSourcing.Data {
    public class EventEntity {
        #region Public Virtual Properties

        public virtual Guid EventID { get; set; }
        public virtual Guid AggregateID { get; set; }
        public virtual int Version { get; set; }
        public virtual DateTimeOffset TimeStamp { get; set; }
        public virtual string Type { get; set; }
        public virtual byte[] Payload { get; set; }

        #endregion

        #region Public Static Methods
        
        public static IEvent Map (IDataRecord record, Func<EventEntity, IEvent> parser) {
            if (record == null) { return null; }
            var result = new EventEntity {
                EventID = record.GetGuidOrDefault (nameof (EventID)).GetValueOrDefault (),
                AggregateID = record.GetGuidOrDefault (nameof (AggregateID)).GetValueOrDefault (),
                Version = record.GetInt32OrDefault (nameof (Version)).GetValueOrDefault (),
                TimeStamp = record.GetDateTimeOffsetOrDefault (nameof (TimeSpan), DateTimeOffset.MinValue).GetValueOrDefault (),
                Type = record.GetStringOrDefault (nameof (Type)),
                Payload = record.GetBlobOrDefault (nameof (Payload))
            };
            return parser (result);
        }

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