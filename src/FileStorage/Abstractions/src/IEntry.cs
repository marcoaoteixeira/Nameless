using System;

namespace Nameless.FileStorage {
    public interface IEntry {

        #region Properties

        /// <summary>
        /// Gets the name of the entry.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the full path of the entry within the file storage.
        /// </summary>
        string Path { get; }

        /// <summary>
        /// Whether if the entry exists or not.
        /// </summary>
        bool Exists { get; }

        /// <summary>
        /// Gets the length of the entry, -1 if it's a directory.
        /// </summary>
        long Length { get; }

        /// <summary>
        /// Gets the date and time in UTC when the entry was last modified.
        /// </summary>
        DateTimeOffset LastWriteTimeUtc { get; }

        #endregion
    }
}