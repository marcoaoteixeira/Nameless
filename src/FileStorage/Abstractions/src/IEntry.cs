using System;
using System.Threading;
using System.Threading.Tasks;

namespace Nameless.FileStorage {
    public interface IEntry {

        #region Properties

        /// <summary>
        /// Gets the path of the entry within the file storage.
        /// </summary>
        string Path { get; }

        /// <summary>
        /// Gets the name of the entry.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets whether the entry exists or not.
        /// </summary>
        bool Exists { get; }

        /// <summary>
        /// Gets the date and time in UTC when the entry was last modified.
        /// </summary>
        DateTimeOffset LastWriteTimeUtc { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Creates a copy of the entry.
        /// </summary>
        /// <param name="destPath">
        /// The destination path of the entry to be created or overwritten.
        /// </param>
        /// <param name="overwrite">
        /// Whether it will overwrite the entry, if exists, or not.
        /// </param>
        /// <param name="token">The cancellation token.</param>
        Task CopyAsync (string destPath, bool overwrite = false, CancellationToken token = default);

        /// <summary>
        /// Moves the entry to another location or renames it.
        /// </summary>
        /// <param name="destPath">
        /// The destination path of the entry to be moved or renamed.
        /// </param>
        /// <param name="token">The cancellation token.</param>
        Task MoveAsync (string destPath, CancellationToken token = default);

        /// <summary>
        /// Deletes the entry, if it exists.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>
        /// <c>true</c> if the entry was deleted; <c>false</c> if not.
        /// </returns>
        Task<bool> DeleteAsync (CancellationToken token = default);

        #endregion
    }
}