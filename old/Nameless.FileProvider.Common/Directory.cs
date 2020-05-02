using System.Collections;
using System.Collections.Generic;
using MS_DirectoryContents = Microsoft.Extensions.FileProviders.IDirectoryContents;

namespace Nameless.FileProvider.Common {
    public sealed class Directory : IDirectory {
        #region Private Read-Only Fields

        private readonly MS_DirectoryContents _directoryContents;

        #endregion

        #region Public Constructors

        public Directory (string path, MS_DirectoryContents directoryContents) {
            Prevent.ParameterNullOrWhiteSpace (path, nameof (path));
            Prevent.ParameterNull (directoryContents, nameof (directoryContents));

            Path = path;
            _directoryContents = directoryContents;
        }

        #endregion

        #region IDirectory Members

        public string Path { get; }

        public bool Exists => _directoryContents.Exists;

        public IEnumerator<IFile> GetEnumerator () => (IEnumerator<IFile>)((IEnumerable)this).GetEnumerator ();

        IEnumerator IEnumerable.GetEnumerator () => new FileEnumerator (_directoryContents);

        #endregion
    }
}