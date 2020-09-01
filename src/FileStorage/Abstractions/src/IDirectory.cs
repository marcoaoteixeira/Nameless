using System;
using System.Collections.Generic;

namespace Nameless.FileStorage {

    /// <summary>
    /// Represents an abstract directory in the file storage.
    /// </summary>
    public interface IDirectory : IEntry {

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
        /// Watchs for changes inside the current directory.
        /// </summary>
        /// <param name="callback">
        /// The callback that will be executed when something happens inside the
        /// diretory.
        /// </param>
        /// <param name="filter">The filter, if any; otherwise "*.*"</param>
        IDisposable Watch (Action<object> callback, string filter = null);

        #endregion
    }
}