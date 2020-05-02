using System;
using System.IO;
using System.Threading.Tasks;
using Nameless.Helpers;

namespace Nameless.FileStorage.FileSystem {
    public class File : IFile {
        #region Private Read-Only Fields

        private readonly string _root;
        private readonly string _path;

        #endregion

        #region Private Fields

        private FileInfo _fileInfo;

        #endregion

        #region Public Constructors

        public File (string root, string path) {
            Prevent.ParameterNullOrWhiteSpace (root, nameof (root));
            Prevent.ParameterNullOrWhiteSpace (path, nameof (path));

            _root = root;
            _path = path;

            var physicalFilePath = PathHelper.GetPhysicalPath (_root, path);
            _fileInfo = new FileInfo (physicalFilePath);
        }

        #endregion

        #region IFile Members

        public string Name => _fileInfo.Name;

        public string DirectoryPath => _path.Substring (startIndex: 0, length: _path.Length - Name.Length).TrimEnd ('/');

        public string Path => _path;

        public bool Exists => _fileInfo.Exists;

        public long Length => _fileInfo.Length;

        public DateTime CreationTimeUtc => _fileInfo.CreationTimeUtc;

        public DateTime LastModifiedUtc => _fileInfo.LastWriteTimeUtc;

        public Task CopyAsync (string destinationPath, bool overwrite = false) {
            if (!Exists) { throw new FileStorageException ("The file does not exist."); }

            var physicalDstPath = PathHelper.GetPhysicalPath (_root, destinationPath);
            if (System.IO.File.Exists (physicalDstPath) && !overwrite) {
                throw new FileStorageException ("Cannot copy file because the destination path already exist.");
            }

            _fileInfo.CopyTo (physicalDstPath, overwrite);

            return Task.CompletedTask;
        }

        public Task<Stream> GetStreamAsync () {
            if (!Exists) { throw new FileStorageException ("The file does not exist."); }

            var stream = _fileInfo.OpenRead ();
            return Task.FromResult<Stream> (stream);
        }

        public Task MoveAsync (string newPath) {
            if (!Exists) { throw new FileStorageException ("The file does not exist."); }

            var physicalNewPath = PathHelper.GetPhysicalPath (_root, newPath);
            if (System.IO.File.Exists (physicalNewPath)) {
                throw new FileStorageException ("Cannot move file because the destination path already exist.");
            }

            _fileInfo.MoveTo (physicalNewPath);

            return Task.CompletedTask;
        }

        public Task<bool> TryDeleteAsync () {
            if (!Exists) { return Task.FromResult (false); }

            _fileInfo.Delete ();

            return Task.FromResult (true);
        }

        #endregion
    }
}