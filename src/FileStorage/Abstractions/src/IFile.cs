using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Nameless.FileStorage {

    /// <summary>
    /// Represents an abstract file in a virtual file storage.
    /// </summary>
    public interface IFile {

        #region Properties

        /// <summary>
        /// Gets the name of the file.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the full path of the entry within the file storage.
        /// </summary>
        string Path { get; }

        /// <summary>
        /// Gets the directory path where the file resides.
        /// </summary>
        string DirectoryPath { get; }

        /// <summary>
        /// Gets whether the file exists or not.
        /// </summary>
        bool Exists { get; }

        /// <summary>
        /// Gets the length of the file, if -1, file does not exists.
        /// </summary>
        long Length { get; }

        /// <summary>
        /// Gets the date and time in UTC when the file was last modified.
        /// </summary>
        DateTimeOffset LastWriteTimeUtc { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Creates a stream to read the contents of a file.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>
        /// An instance of <see cref="Stream"/> that can be used to read the
        /// contents of the file. The caller must close the stream when
        /// finished.
        /// </returns>
        Task<Stream> CreateStreamAsync (CancellationToken token = default);

        /// <summary>
        /// Creates a copy of the current file.
        /// </summary>
        /// <param name="destFilePath">
        /// The root relative path of the destination file to be created or
        /// overwritten.
        /// </param>
        /// <param name="overwrite">
        /// Whether it will overwrite the file, if exists, or not.
        /// </param>
        /// <param name="token">The cancellation token.</param>
        Task CopyAsync (string destFilePath, bool overwrite = false, CancellationToken token = default);

        /// <summary>
        /// Moves the current file to another location or renames it.
        /// </summary>
        /// <param name="destFilePath">
        /// The root relative path of the file after the move/rename.
        /// </param>
        /// <param name="token">The cancellation token.</param>
        Task MoveAsync (string destFilePath, CancellationToken token = default);

        /// <summary>
        /// Deletes the current file, if it exists.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>
        /// <c>true</c> if the file was deleted; <c>false</c> if not.
        /// </returns>
        Task<bool> DeleteAsync (CancellationToken token = default);

        /// <summary>
        /// Watchs for changes in the current file.
        /// </summary>
        IDisposable Watch (Action<ChangeEventArgs> callback);

        #endregion
    }
}