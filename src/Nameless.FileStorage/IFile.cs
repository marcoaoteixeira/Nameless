using System;
using System.IO;
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
        /// Gets the full path of the file's containing directory within the file storage.
        /// </summary>
        string DirectoryPath { get; }

        /// <summary>
        /// Gets the full path of the file within the file storage.
        /// </summary>
        string Path { get; }

        /// <summary>
        /// Whether if the file exists or not.
        /// </summary>
        bool Exists { get; }
        
        /// <summary>
        /// Gets the length of the file.
        /// </summary>
        long Length { get; }

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
        /// Creates a copy of a file in the file storage.
        /// </summary>
        /// <param name="destinationPath">The path of the destination file to be created.</param>
        /// <param name="overwrite">Whether it will overwrite the file, if exists, or not.</param>
        /// <exception cref="FileStoreException">Thrown if the specified file does not exist or if the <paramref name="destinationPath"/> path already exists.</exception>
        Task CopyAsync (string destinationPath, bool overwrite = false);

        /// <summary>
        /// Creates a stream to read the contents of a file in the file storage.
        /// </summary>
        /// <returns>An instance of <see cref="Stream"/> that can be used to read the contents of the file. The caller must close the stream when finished.</returns>
        /// <exception cref="FileStoreException">Thrown if the specified file does not exist.</exception>
        Task<Stream> GetStreamAsync ();

        /// <summary>
        /// Renames or moves a file to another location in the file storage.
        /// </summary>
        /// <param name="newPath">The new path of the file after the rename/move.</param>
        /// <exception cref="FileStorageException">Thrown if the specified file does not exist or if the <paramref name="newPath"/> path already exists.</exception>
        Task MoveAsync (string newPath);

        /// <summary>
        /// Deletes a file in the file storage, if it exists.
        /// </summary>
        /// <returns><c>true</c> if the file was deleted; <c>false</c> if not.</returns>
        Task<bool> TryDeleteAsync ();

        #endregion
    }
}