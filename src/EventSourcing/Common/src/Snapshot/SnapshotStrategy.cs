using System;
using System.Linq;
using Nameless.EventSourcing.Domain;

namespace Nameless.EventSourcing.Snapshot {
    public class SnapshotStrategy : ISnapshotStrategy {
        #region Private Read-Only Properties

        private EventSourcingSettings Settings { get; }

        #endregion

        #region Public Constructors

        public SnapshotStrategy (EventSourcingSettings settings) {
            Prevent.ParameterNull (settings, nameof (settings));

            Settings = settings;
        }

        #endregion

        #region ISnapshotStrategy Members

        public bool IsSnapshottable (Type aggregateType) {
            Prevent.ParameterNull (aggregateType, nameof (aggregateType));

            if (typeof (Snapshottable).IsAssignableFrom (aggregateType)) { return true; }

            return IsSnapshottable (aggregateType.BaseType);
        }

        public bool ShouldMakeSnapshot (AggregateRoot aggregate) {
            if (!Settings.TakeSnapshots) { return false; }

            var totalEventsToCommit = aggregate.GetUncommittedEvents().Count ();
            
            var currentVersionMustBeGreaterOrEqualToSnapshotFrequency = aggregate.CurrentVersion >= Settings.SnapshotFrequency;
            var totalEventsToCommitMustBeGreaterOrEqualToSnapshotFrequency = totalEventsToCommit >= Settings.SnapshotFrequency;
            var modOfCurrentVersionAndSnapshotFrequencyMustBeLowerThenTotalEventsToCommit = (aggregate.CurrentVersion % Settings.SnapshotFrequency) < totalEventsToCommit;
            var modOfCurrentVersionAndSnapshotFrequencyMustBeZero = (aggregate.CurrentVersion % Settings.SnapshotFrequency) == 0;

            return currentVersionMustBeGreaterOrEqualToSnapshotFrequency && (
                totalEventsToCommitMustBeGreaterOrEqualToSnapshotFrequency ||
                modOfCurrentVersionAndSnapshotFrequencyMustBeLowerThenTotalEventsToCommit ||
                modOfCurrentVersionAndSnapshotFrequencyMustBeZero
            );
        }

        #endregion
    }
}