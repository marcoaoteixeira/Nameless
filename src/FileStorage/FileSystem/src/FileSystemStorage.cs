using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using Nameless.Helpers;
using SysDirectory = System.IO.Directory;
using SysFile = System.IO.File;
using SysPath = System.IO.Path;
using SysStream = System.IO.Stream;

namespace Nameless.FileStorage.FileSystem {

    public class FileSystemFileStorage : IFileStorage, IDisposable {
        #region Private Read-Only Fields

        private readonly FileSystemStorageSettings _settings;

        #endregion

        #region Private Fields

        private PhysicalFileProvider _fileProvider;
        private bool _disposed;

        #endregion

        #region Private Properties

        private string Root { get; }

        #endregion

        #region Public Constructors

        public FileSystemFileStorage (FileSystemStorageSettings settings = null) {
            Prevent.ParameterNull (settings, nameof (settings));

            _settings = settings ?? new FileSystemStorageSettings ();

            Root = PathHelper.Normalize (_settings.Root);

            Initialize ();
        }

        #endregion

        #region Destructor

        ~FileSystemFileStorage () {
            Dispose (disposing: false);
        }

        #endregion

        #region Private Methods

        private void Initialize () {
            _fileProvider = new PhysicalFileProvider (Root);
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

        private IDisposable ChangeWatcherFactory (string filter, Action callback) {
            BlockAccessAfterDispose ();

            return ChangeToken.OnChange (
                changeTokenProducer: () => {
                    var changeToken = _fileProvider.Watch (filter);
                    if (changeToken is NullChangeToken) {
                        throw new InvalidOperationException ("Invalid filters");
                    }
                    return changeToken;
                },
                changeTokenConsumer : callback
            );
        }

        #endregion

        #region IFileStorage Members

        /// <inheritdoc />
        public Task<bool> CreateDirectoryAsync (string relativePath, CancellationToken token = default) {
            BlockAccessAfterDispose ();

            relativePath = PathHelper.Normalize (relativePath);

            var directoryPath = PathHelper.GetPhysicalPath (Root, relativePath);
            if (SysDirectory.Exists (directoryPath)) {
                return Task.FromResult (false);
            }

            SysDirectory.CreateDirectory (directoryPath);

            return Task.FromResult (true);
        }

        /// <inheritdoc />
        /// <exception cref="FileStorageException">
        /// Thrown if the specified path does not points to a directory.
        /// </exception>
        public Task<IDirectory> GetDirectoryAsync (string relativePath) {
            BlockAccessAfterDispose ();

            relativePath = PathHelper.Normalize (relativePath);

            var directoryPath = PathHelper.GetPhysicalPath (Root, relativePath);
            if (!SysDirectory.Exists (directoryPath)) {
                throw new FileStorageException ("The specified path does not points to a directory.");
            }

            IDirectory directory = new Directory (Root, relativePath, ChangeWatcherFactory);
            return Task.FromResult (directory);
        }

        /// <inheritdoc />
        public Task CreateFileAsync (string relativePath, SysStream input, bool overwrite = false, CancellationToken token = default) {
            Prevent.ParameterNull (input, nameof (input));
            
            BlockAccessAfterDispose ();

            relativePath = PathHelper.Normalize (relativePath);

            var filePath = PathHelper.GetPhysicalPath (Root, relativePath);
            if (SysFile.Exists (filePath) && !overwrite) {
                throw new FileStorageException ("Cannot create file because the destination path already exists.");
            }

            // Create directory path if it doesn't exist.
            var directoryPath = SysPath.GetDirectoryName (filePath);
            SysDirectory.CreateDirectory (directoryPath);

            using var output = SysFile.Create (filePath);
            return input.CopyToAsync (output);
        }

        /// <inheritdoc />
        public Task<IFile> GetFileAsync (string relativePath) {
            BlockAccessAfterDispose ();

            relativePath = PathHelper.Normalize (relativePath);

            var file = new File (Root, relativePath, ChangeWatcherFactory);

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