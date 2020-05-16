namespace Nameless.EventSourcing {
    public class EventSourcingSettings {

        #region Public Properties

        /// <summary>
        /// Whether will take snapshots of an aggregate. Default value is
        /// <c>false</c>.
        /// </summary>
        public bool TakeSnapshots { get; set; } = false;

        /// <summary>
        /// Gets or sets the snapshot frequency. Default value is 10.
        /// </summary>
        public int SnapshotFrequency { get; set; } = 10;

        #endregion
    }
}