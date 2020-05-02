using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nameless.FileStorage {
    /// <summary>
    /// Represents an abstract directory in a virtual file storage.
    /// </summary>
    public interface IDirectory {
        #region Properties

        /// <summary>
        /// Gets the name of the file.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the full path of the file within the file storage.
        /// </summary>
        string Path { get; }

        /// <summary>
        /// Whether if the file exists or not.
        /// </summary>
        bool Exists { get; }
        
        /// <summary>
        /// Gets the date that the file was created.
        /// </summary>
        DateTime CreationTimeUtc { get; }

        /// <summary>
        /// Gets the date and time in UTC when the file was last modified.
        /// </summary>
        DateTime LastModifiedUtc { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Enumerates the content (files) in a given directory within the file storage.
        /// </summary>
        /// <param name="includeSubDirectories">A flag to indicate whether to get the contents from just the top directory or from all sub-directories as well.</param>
        /// <returns>The list of files in the given directory.</returns>
        Task<IEnumerable<IFile>> GetContentAsync (bool includeSubDirectories = false);

        /// <summary>
        /// Enumerates the directories below the current directory within the file storage.
        /// </summary>
        /// <param name="includeSubDirectories">A flag to indicate whether to get the directories from just the top directory or from all sub-directories as well.</param>
        /// <returns>The list of files in the given directory.</returns>
        Task<IEnumerable<IDirectory>> GetSubDirectoriesAsync (bool includeSubDirectories = false);

        /// <summary>
        /// Deletes a directory in the file storage, if it exists.
        /// </summary>
        /// <returns><c>true</c> if the directory was deleted; <c>false</c> if not.</returns>
        Task<bool> TryDeleteAsync ();

        #endregion
    }
}