namespace Nameless.FileStorage {
    public sealed class ChangeEventArgs : EventArgs {
        #region Public Properties

        public string OriginalPath { get; set; } = default!;
        public string? CurrentPath { get; set; }
        public ChangeReason Reason { get; set; }

        #endregion

        #region Public Static Properties

        public new static ChangeEventArgs Empty => new();

        #endregion
    }

    public enum ChangeReason {
        None = 0,

        Changed = 1,

        Created = 2,

        Deleted = 4,

        Renamed = 8
    }
}