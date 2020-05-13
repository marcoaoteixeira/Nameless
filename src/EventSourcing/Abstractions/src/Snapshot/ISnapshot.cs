using System;

namespace Nameless.EventSourcing.Snapshot {
    public interface ISnapshot {
        #region Properties
        
        Guid AggregateID { get; }
        int Version { get; }

        #endregion
    }
}