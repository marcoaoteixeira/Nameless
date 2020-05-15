using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;

namespace Nameless.FileStorage {

    /// <summary>
    /// Represents an abstract directory in the file storage.
    /// </summary>
    public interface IDirectory {

        #region Properties

        /// <summary>
        /// Gets the name of the directory.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the full path of the directory.
        /// </summary>
        string Path { get; }

        /// <summary>
        /// Gets whether the directory exists or not.
        /// </summary>
        bool Exists { get; }

        /// <summary>
        /// Gets the date and time in UTC when the directory was last modified.
        /// </summary>
        DateTimeOffset LastWriteTimeUtc { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Enumerates the files inside the current directory.
        /// </summary>
        /// <param name="includeSubDirectories">
        /// A flag to indicate whether to get the files from just the top
        /// directory or from all sub-directories as well.
        /// </param>
        /// <returns>The list of files in the given directory.</returns>
        IAsyncEnumerable<IFile> GetFilesAsync (bool includeSubDirectories = false);

        /// <summary>
        /// Enumerates the directories below the current directory.
        /// </summary>
        /// <param name="includeSubDirectories">
        /// A flag to indicate whether to get the directories from just the top
        /// directory or from all sub-directories as well.
        /// </param>
        /// <returns>The list of files in the given directory.</returns>
        IAsyncEnumerable<IDirectory> GetDirectoriesAsync (bool includeSubDirectories = false);

        /// <summary>
        /// Deletes the current directory.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the directory was deleted; <c>false</c> if not.
        /// </returns>
        Task<bool> DeleteAsync ();

        /// <summary>
        /// Watchs for changes inside the current directory.
        /// </summary>
        /// <param name="filter">The filter, if any; otherwise "*.*"</param>
        IDisposable Watch (Action<ChangeEventArgs> callback, string filter = null);

        #endregion
    }
}