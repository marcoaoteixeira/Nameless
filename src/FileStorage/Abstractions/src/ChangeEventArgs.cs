using System;

namespace Nameless.FileStorage {
    public sealed class ChangeEventArgs : EventArgs {
        #region Public Properties

        public string OldPath { get; set; }
        public string NewPath { get; set; }
        public ChangeEventAction Action { get; set; }

        #endregion

        #region Public Static Properties

        public new static ChangeEventArgs Empty => new ChangeEventArgs ();
            
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