using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nameless.FileStorage {
    /// <summary>
    /// Represents an abstract directory in a virtual file storage.
    /// </summary>
    public interface IDirectory : IEntry {
        
        #region Methods

        /// <summary>
        /// Enumerates the files in a given directory within the
        /// file storage.
        /// </summary>
        /// <param name="includeSubDirectories">
        /// A flag to indicate whether to get the contents from just the top
        /// directory or from all sub-directories as well.
        /// </param>
        /// <returns>The list of files in the given directory.</returns>
        IAsyncEnumerable<IFile> GetFilesAsync (bool includeSubDirectories = false);

        /// <summary>
        /// Enumerates the directories below the current directory within the
        /// file storage.
        /// </summary>
        /// <param name="includeSubDirectories">
        /// A flag to indicate whether to get the directories from just the top
        /// directory or from all sub-directories as well.
        /// </param>
        /// <returns>The list of files in the given directory.</returns>
        IAsyncEnumerable<IDirectory> GetDirectoriesAsync (bool includeSubDirectories = false);

        /// <summary>
        /// Deletes a directory in the file storage, if it exists.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the directory was deleted; <c>false</c> if not.
        /// </returns>
        Task<bool> DeleteAsync ();

        /// <summary>
        /// Watchs for changes inside this directory.
        /// </summary>
        /// <param name="filters">The filters, if any.</param>
        void Watch (Action<ChangeEventArgs> callback, string filters = null);

        #endregion
    }
}