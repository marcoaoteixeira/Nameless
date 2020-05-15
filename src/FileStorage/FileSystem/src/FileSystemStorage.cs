using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace Nameless.FileStorage.FileSystem {
    public class FileSystemStorage : IFileStorage, IDisposable {
        #region Private Read-Only Fields

        private readonly FileStorageSettings _settings;
        private readonly string _root;

        #endregion

        #region Private Fields

        private PhysicalFileProvider _fileProvider;
        private bool _disposed;

        #endregion

        #region Public Constructors

        public FileSystemStorage (FileStorageSettings settings) {
            Prevent.ParameterNull (settings, nameof (settings));

            _settings = settings ?? new FileStorageSettings ();
            _root = PathHelper.Normalize (_settings.Root);

            Initialize ();
        }

        #endregion

        #region Destructor

        ~FileSystemStorage () {
            Dispose (disposing: false);
        }

        #endregion

        #region Private Methods

        private void Initialize () {
            _fileProvider = new PhysicalFileProvider (_root);
        }

        private void Dispose (bool disposing) {
            if (_disposed) { return; }
            if (disposing) {
                if (_fileProvider != null) {
                    _fileProvider.Dispose ();
                }
            }

            _fileProvider = null;
            _disposed = true;
        }

        private void BlockAccessAfterDispose () {
            if (_disposed) {
                throw new ObjectDisposedException (GetType ().FullName);
            }
        }

        private IChangeToken ChangeTokenFactory (string filters) {
            BlockAccessAfterDispose ();

            var changeToken = _fileProvider.Watch (filters);
            if (changeToken is NullChangeToken) {
                throw new InvalidOperationException ("Invalid filters");
            }
            return changeToken;
        }

        #endregion

        #region IFileStorage Members

        /// <inheritdoc />
        public Task<bool> CreateDirectoryAsync (string relativePath) {
            BlockAccessAfterDispose ();

            relativePath = PathHelper.Normalize (relativePath);

            var directoryPath = PathHelper.GetPhysicalPath (_root, relativePath);
            if (System.IO.Directory.Exists (directoryPath)) {
                return Task.FromResult (false);
            }

            System.IO.Directory.CreateDirectory (directoryPath);

            return Task.FromResult (true);
        }

        /// <inheritdoc />
        /// <exception cref="FileStorageException">
        /// Thrown if the specified path does not points to a directory.
        /// </exception>
        public Task<IDirectory> GetDirectoryAsync (string relativePath) {
            BlockAccessAfterDispose ();

            relativePath = PathHelper.Normalize (relativePath);

            var directoryPath = PathHelper.GetPhysicalPath (_root, relativePath);
            if (!System.IO.Directory.Exists (directoryPath)) {
                throw new FileStorageException ("The specified path does not points to a directory.");
            }

            IDirectory directory = new Directory (_root, relativePath, ChangeTokenFactory);
            return Task.FromResult (directory);
        }

        /// <inheritdoc />
        public Task CreateFileAsync (string relativePath, Stream input, bool overwrite = false) {
            BlockAccessAfterDispose ();

            relativePath = PathHelper.Normalize (relativePath);

            var filePath = PathHelper.GetPhysicalPath (_root, relativePath);
            if (System.IO.File.Exists (filePath) && !overwrite) {
                throw new FileStorageException ("Cannot create file because the destination path already exist.");
            }

            // Create directory path if it doesn't exist.
            var directoryPath = Path.GetDirectoryName (filePath);
            System.IO.Directory.CreateDirectory (directoryPath);

            using var output = System.IO.File.Create (filePath);
            return input.CopyToAsync (output);
        }

        /// <inheritdoc />
        public Task<IFile> GetFileAsync (string relativePath) {
            BlockAccessAfterDispose ();

            relativePath = PathHelper.Normalize (relativePath);

            var file = new File (_root, relativePath, ChangeTokenFactory);

            return Task.FromResult<IFile> (file);
        }

        #endregion

        #region IDisposable Members

        public void Dispose () {
            Dispose (disposing: true);
            GC.SuppressFinalize (this);
        }

        #endregion
    }
}