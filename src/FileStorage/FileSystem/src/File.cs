using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;
using Nameless.FileStorage.FileSystem.Properties;

namespace Nameless.FileStorage.FileSystem {
    public sealed class File : IFile {

        #region Private Properties

        private string Root { get; }
        private FileInfo CurrentFile { get; }
        private Func<string, IChangeToken> ChangeTokenFactory { get; }

        #endregion

        #region Public Constructors

        public File (string root, string path, Func<string, IChangeToken> changeTokenFactory) {
            Prevent.ParameterNullOrWhiteSpace (root, nameof (root));
            Prevent.ParameterNullOrWhiteSpace (path, nameof (path));
            Prevent.ParameterNull (changeTokenFactory, nameof (changeTokenFactory));

            Root = root;
            Path = path;
            CurrentFile = new FileInfo (PathHelper.GetPhysicalPath (root, path));
            ChangeTokenFactory = changeTokenFactory;
        }

        #endregion

        #region IFile Members

        /// <inheritdoc />
        public string Name => CurrentFile.Name;

        /// <inheritdoc />
        public string DirectoryPath => Path.Substring (startIndex: 0, length: Path.Length - Name.Length).TrimEnd ('/');

        /// <inheritdoc />
        public string Path { get; }

        /// <inheritdoc />
        public bool Exists => CurrentFile.Exists;

        /// <inheritdoc />
        public long Length => CurrentFile.Length;

        /// <inheritdoc />
        public DateTimeOffset LastWriteTimeUtc => CurrentFile.LastWriteTimeUtc;

        /// <inheritdoc />
        /// <exception cref="FileStorageException">
        /// Thrown if the specified file does not exist.
        /// </exception>
        public Task<Stream> CreateStreamAsync () {
            if (!Exists) { throw new FileStorageException (Resources.TheFileDoesNotExistsMessage); }

            // We are setting buffer size to 1 to prevent FileStream from
            // allocating it's internal buffer 0 causes constructor to throw
            var bufferSize = 1;
            var stream = new FileStream (
                path: CurrentFile.FullName,
                mode: FileMode.Open,
                access: FileAccess.Read,
                share: FileShare.ReadWrite,
                bufferSize: bufferSize,
                options: FileOptions.Asynchronous | FileOptions.SequentialScan
            );
            return Task.FromResult<Stream> (stream);
        }

        /// <inheritdoc />
        /// <exception cref="FileStorageException">
        /// Thrown if the specified file does not exist or if the
        /// <paramref name="destFilePath"/> path already exists.
        /// </exception>
        public Task CopyAsync (string destFilePath, bool overwrite = false) {
            if (!Exists) { throw new FileStorageException (Resources.TheFileDoesNotExistsMessage); }

            var destFileName = PathHelper.GetPhysicalPath (Root, destFilePath);
            if (System.IO.File.Exists (destFileName) && !overwrite) {
                throw new FileStorageException ("Cannot copy file because the destination path already exists.");
            }

            CurrentFile.CopyTo (destFileName, overwrite);

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        /// <exception cref="FileStorageException">
        /// Thrown if the specified file does not exist or if the
        /// <paramref name="destFilePath"/> path already exists.
        /// </exception>
        public Task MoveAsync (string destFilePath) {
            if (!Exists) { throw new FileStorageException (Resources.TheFileDoesNotExistsMessage); }

            var destFileName = PathHelper.GetPhysicalPath (Root, destFilePath);
            if (System.IO.File.Exists (destFileName)) {
                throw new FileStorageException ("Cannot move file because the destination path already exists.");
            }

            CurrentFile.MoveTo (destFileName);

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task<bool> DeleteAsync () {
            if (!Exists) { return Task.FromResult (false); }

            CurrentFile.Delete ();

            return Task.FromResult (true);
        }

        /// <inheritdoc />
        public void Watch (Action<ChangeEventArgs> callback) {
            ChangeToken.OnChange (
                changeTokenProducer: () => ChangeTokenFactory (Path),
                changeTokenConsumer: (state) => callback (new ChangeEventArgs { Path = state }),
                state : Path
            );
        }

        #endregion
    }
}