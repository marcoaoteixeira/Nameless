using System.IO;
using Nameless.IO.FileSystem;

namespace Nameless.WPF;

public static class FileSystemProviderExtensions {
    extension(IFileSystemProvider self) {
        public IDirectory GetBackupDirectory() {
            return self.InnerGetDirectory(
                WPFConstants.FolderStructure.BackupDirectoryName
            );
        }

        public IDirectory GetDatabaseDirectory() {
            return self.InnerGetDirectory(
                WPFConstants.FolderStructure.DatabaseDirectoryName
            );
        }

        public IDirectory GetTemporaryDirectory() {
            return self.InnerGetDirectory(
                WPFConstants.FolderStructure.TemporaryDirectoryName
            );
        }

        public IDirectory GetTemporaryDirectory(DateTimeOffset timestamp) {
            return self.InnerGetDirectory(
                Path.Combine(
                    WPFConstants.FolderStructure.TemporaryDirectoryName,
                    $"{timestamp:yyyyMMddHHmmss}"
                )
            );
        }

        public IDirectory GetUpdateDirectory() {
            return self.InnerGetDirectory(
                WPFConstants.FolderStructure.UpdateDirectoryName
            );
        }

        private IDirectory InnerGetDirectory(string relativePath) {
            return self.GetDirectory(relativePath);
        }
    }
}