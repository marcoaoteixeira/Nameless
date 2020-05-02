using System.IO;
using MS_FileInfo = Microsoft.Extensions.FileProviders.IFileInfo;

namespace Nameless.FileProvider.Common {
    public sealed class File : IFile {
        #region Private Read-Only Fields

        private readonly MS_FileInfo _fileInfo;

        #endregion

        #region Public Constructors

        public File (MS_FileInfo fileInfo) {
            Prevent.ParameterNull (fileInfo, nameof (fileInfo));

            _fileInfo = fileInfo;
        }

        #endregion

        #region IFile Members

        public bool Exists => _fileInfo.Exists;

        public long Length => _fileInfo.Length;

        public string Path => _fileInfo.PhysicalPath;

        public Stream GetStream () => _fileInfo.CreateReadStream ();

        #endregion
    }
}