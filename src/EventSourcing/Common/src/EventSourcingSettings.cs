namespace Nameless.EventSourcing {
    public class EventSourcingSettings {
        #region Public Properties

        public bool TakeSnapshots { get; set; } = false;
        public int SnapshotFrequency { get; set; } = 10;

        #endregion
    }
}