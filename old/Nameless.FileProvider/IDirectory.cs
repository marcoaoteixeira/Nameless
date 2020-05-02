using System.Collections.Generic;

namespace Nameless.FileProvider {
    public interface IDirectory : IEnumerable<IFile> {
        #region Properties

        string Path { get; }
        bool Exists { get; }

        #endregion
    }
}