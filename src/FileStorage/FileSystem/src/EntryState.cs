namespace Nameless.FileStorage.FileSystem {
    internal class EntryState {
        #region Internal Properties

        internal string OldPath { get; set; }
        internal string NewPath { get; set; }
        internal ChangeEventAction Action { get; set; }

        #endregion

        #region Internal Methods

        internal ChangeEventArgs ToChangeEventArgs () {
            return new ChangeEventArgs {
                OldPath = OldPath,
                    NewPath = NewPath,
                    Action = Action
            };
        }

        #endregion
    }
}