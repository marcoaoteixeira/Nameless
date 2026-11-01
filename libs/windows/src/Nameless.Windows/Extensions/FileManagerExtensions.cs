using Nameless.IO;

namespace Nameless.Windows;

/// <summary>
///     File Manager Extension Methods
/// </summary>
public static class FileManagerExtensions {
    extension(IFileManager self) {
        /// <summary>
        ///     Retrieves the application backup directory.
        /// </summary>
        /// <returns>
        ///     The backup directory.
        /// </returns>
        public IDirectory GetBackupDirectory() {
            return self.InnerGetDirectory("backups");
        }

        /// <summary>
        ///     Retrieves the application database directory.
        /// </summary>
        /// <returns>
        ///     The database directory.
        /// </returns>
        public IDirectory GetDatabaseDirectory() {
            return self.InnerGetDirectory("databases");
        }

        /// <summary>
        ///     Retrieves the application temporary directory.
        /// </summary>
        /// <param name="timestamp">
        ///     A timestamp that translate into a subdirectory name.
        /// </param>
        /// <returns>
        ///     The temporary directory.
        /// </returns>
        public IDirectory GetTemporaryDirectory(DateTimeOffset? timestamp = null) {
            const string Name = "temporary";

            var relativePath = timestamp is not null
                ? SysPath.Combine(Name, $"{timestamp:yyyyMMddHHmmss}")
                : Name;

            return self.InnerGetDirectory(relativePath);
        }

        /// <summary>
        ///     Retrieves the application update directory.
        /// </summary>
        /// <returns>
        ///     The update directory.
        /// </returns>
        public IDirectory GetUpdateDirectory() {
            return self.InnerGetDirectory("updates");
        }

        private IDirectory InnerGetDirectory(string relativePath) {
            return self.GetDirectory(relativePath);
        }
    }
}