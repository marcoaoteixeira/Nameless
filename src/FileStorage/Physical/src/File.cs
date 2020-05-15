using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;
using Nameless.FileStorage.Physical.Properties;

namespace Nameless.FileStorage.Physical {
    public sealed class File : IFile {

        #region Private Properties

        private string Root { get; }
        private Func<string, IChangeToken> ChangeTokenFactory { get; }
        private FileInfo CurrentFile { get; set; }
        private EntryState CurrentState { get; } = new EntryState ();

        #endregion

        #region Public Constructors

        public File (string root, string path, Func<string, IChangeToken> changeTokenFactory) {
            Prevent.ParameterNullOrWhiteSpace (root, nameof (root));
            Prevent.ParameterNullOrWhiteSpace (path, nameof (path));
            Prevent.ParameterNull (changeTokenFactory, nameof (changeTokenFactory));

            Root = root;
            ChangeTokenFactory = changeTokenFactory;

            FetchFileInfo (PathHelper.GetPhysicalPath (root, path));
        }

        #endregion

        #region Private Methods

        private void FetchFileInfo (string path) {
            CurrentFile = !string.IsNullOrWhiteSpace (path) ? new FileInfo (path) : null;
        }

        #endregion

        #region IFile Members

        /// <inheritdoc />
        public string Name => CurrentFile.Name;

        /// <inheritdoc />
        public string Path => CurrentFile.FullName[Root.Length..].TrimStart (System.IO.Path.DirectorySeparatorChar);

        /// <inheritdoc />
        public string DirectoryPath => CurrentFile.DirectoryName[Root.Length..].TrimStart (System.IO.Path.DirectorySeparatorChar);

        /// <inheritdoc />
        public bool Exists => CurrentFile.Exists;

        /// <inheritdoc />
        public long Length => CurrentFile.Exists ? CurrentFile.Length : -1;

        /// <inheritdoc />
        public DateTimeOffset LastWriteTimeUtc => CurrentFile.LastWriteTimeUtc;

        /// <inheritdoc />
        /// <exception cref="FileStorageException">
        /// Thrown if the current file does not exist.
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
        /// <exception cref="FileNotFoundException">
        /// Thrown if the specified current file does not exist.
        /// </exception>
        /// <exception cref="IOException">
        /// An error occurs, or the destination file already exists and
        /// overwrite is <c>false</c> - or - trying to copy to the same
        /// destination as the current file.
        /// </exception>
        /// <exception cref="System.Security.SecurityException">
        /// The caller does not have the required permission.
        /// </exception>
        /// <exception cref="DirectoryNotFoundException">
        /// The directory specified in destFileName does not exist.
        /// </exception>
        /// <exception cref="UnauthorizedAccessException">
        /// A directory path is passed in, or the file is being moved to
        /// a different drive.
        /// </exception>
        /// <exception cref="PathTooLongException">
        /// The specified path, file name, or both exceed the system-defined
        /// maximum length.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// <paramref name="destFilePath" /> contains a colon (:) in the middle
        /// of the <c>string</>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="destFilePath" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="destFilePath" /> is empty or white spaces.
        /// </exception>
        public Task CopyAsync (string destFilePath, bool overwrite = false) {
            Prevent.ParameterNullOrWhiteSpace (destFilePath, nameof (destFilePath));

            var destination = PathHelper.GetPhysicalPath (Root, destFilePath);

            CurrentFile.CopyTo (destination, overwrite);

            // Track changes
            CurrentState.OldPath = CurrentFile.FullName;
            CurrentState.NewPath = destination;
            CurrentState.Action = ChangeEventAction.Copied;
            // Track changes

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        /// <exception cref="IOException">
        /// An I/O error occurs, such as the destination file already exists or
        /// the destination device is not ready.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="destFilePath" /> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="destFilePath" /> is empty, contains only white
        /// spaces, or contains invalid characters.
        /// </exception>
        /// <exception cref="System.Security.SecurityException">
        /// The caller does not have the required permission.
        /// </exception>
        /// <exception cref="UnauthorizedAccessException">
        /// <paramref name="destFilePath" /> is read-only or is a directory.
        /// </exception>
        /// <exception cref="FileNotFoundException">
        /// The current file is not found.
        /// </exception>
        /// <exception cref="DirectoryNotFoundException">
        /// The specified path is invalid, such as being on an unmapped drive.
        /// </exception>
        /// <exception cref="PathTooLongException">
        /// The specified path, file name, or both exceed the system-defined
        /// maximum length.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// <paramref name="destFilePath" /> contains a colon (:) in the
        /// middle of the <c>string</c>.
        /// </exception>
        public Task MoveAsync (string destFilePath) {
            Prevent.ParameterNullOrWhiteSpace (destFilePath, nameof (destFilePath));

            var destination = PathHelper.GetPhysicalPath (Root, destFilePath);

            CurrentFile.MoveTo (destination);

            // Track changes
            CurrentState.OldPath = CurrentFile.FullName;
            CurrentState.NewPath = destination;
            CurrentState.Action = ChangeEventAction.Moved;
            // Track changes

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        /// <exception cref="FileNotFoundException">
        /// The current file could not be found.
        /// </exception>
        /// <exception cref="IOException">
        /// An I/O error occurred while opening the current file.
        /// </exception>
        /// <exception cref="UnauthorizedAccessException">
        /// The current file is read-only. -or- This operation is not supported
        /// on the current platform. -or- The caller does not have the required
        /// permission.
        /// </exception>
        public Task<bool> DeleteAsync () {
            if (!Exists) { return Task.FromResult (false); }

            CurrentFile.Delete ();

            // Track changes
            CurrentState.OldPath = CurrentFile.FullName;
            CurrentState.NewPath = null;
            CurrentState.Action = ChangeEventAction.Deleted;
            // Track changes

            return Task.FromResult (true);
        }

        /// <inheritdoc />
        public IDisposable Watch (Action<ChangeEventArgs> callback) {
            return ChangeToken.OnChange (
                changeTokenProducer: () => ChangeTokenFactory (Path),
                changeTokenConsumer: (state) => {
                    callback (state.ToChangeEventArgs ());
                    FetchFileInfo (state.NewPath ?? state.OldPath);
                },
                state : CurrentState
            );
        }

        #endregion
    }
}