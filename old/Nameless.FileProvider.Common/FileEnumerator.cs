using System;
using System.Collections;
using System.Collections.Generic;
using MS_FileInfo = Microsoft.Extensions.FileProviders.IFileInfo;

namespace Nameless.FileProvider.Common {
    public sealed class FileEnumerator : IEnumerator<IFile> {
        #region Private Fields

        private IEnumerator<MS_FileInfo> _enumerator;
        private bool _disposed;

        #endregion

        #region Public Constructors

        public FileEnumerator (IEnumerable<MS_FileInfo> collection) {
            Prevent.ParameterNull (collection, nameof (collection));

            _enumerator = collection.GetEnumerator ();
        }

        #endregion

        #region Destructor

        ~FileEnumerator () {
            Dispose (disposing: false);
        }

        #endregion

        #region Private Methods

        private void Dispose (bool disposing) {
            if (_disposed) { return; }
            if (disposing) {
                if (_enumerator != null) {
                    _enumerator.Dispose ();
                }
            }

            _enumerator = null;
            _disposed = true;
        }

        #endregion

        #region IEnumerator<IFile> Members

        public IFile Current => (IFile)((IEnumerator)this).Current;

        object IEnumerator.Current => new File (_enumerator.Current);

        public void Dispose () {
            Dispose (disposing: true);
            GC.SuppressFinalize (this);
        }

        public bool MoveNext () => _enumerator.MoveNext ();

        public void Reset () => _enumerator.Reset ();

        #endregion
    }
}