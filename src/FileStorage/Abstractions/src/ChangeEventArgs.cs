using System;

namespace Nameless.FileStorage {
    public sealed class ChangeEventArgs : EventArgs {
        #region Public Properties

        public string Path { get; set; }

        #endregion
    }
}