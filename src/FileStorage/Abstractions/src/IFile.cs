using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Nameless.FileStorage {

    /// <summary>
    /// Represents an abstract file in a virtual file storage.
    /// </summary>
    public interface IFile : IEntry {

        #region Properties

        /// <summary>
        /// Gets the directory path where the file resides.
        /// </summary>
        string DirectoryPath { get; }

        /// <summary>
        /// Gets the length of the file.
        /// </summary>
        long Length { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Opens a stream to read the contents of a file.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>
        /// An instance of <see cref="Stream"/> that can be used to read the
        /// contents of the file. The caller must close the stream when
        /// finished.
        /// </returns>
        Task<Stream> OpenAsync (CancellationToken token = default);

        /// <summary>
        /// Watchs for changes to the file.
        /// </summary>
        /// <param name="callback">The change callback.</param>
        IDisposable Watch (Action<object> callback);

        #endregion
    }
}