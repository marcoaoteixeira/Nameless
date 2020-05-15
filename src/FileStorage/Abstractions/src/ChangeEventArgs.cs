using System;

namespace Nameless.FileStorage {
    public sealed class ChangeEventArgs : EventArgs {
        #region Public Properties

        public string OldPath { get; set; }
        public string NewPath { get; set; }
        public ChangeEventAction Action { get; set; }

        #endregion
    }

    public enum ChangeEventAction : int {
        None,

        Modified,

        Deleted,

        Renamed,

        Moved,

        Copied
    }
}