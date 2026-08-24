using System.IO;
using Nameless.IO;
using Nameless.IO.Explorer;

namespace Nameless.Windows;

public static class FileSystemProviderExtensions {
    extension(IFileExplorer self) {
        public IDirectory GetBackupDirectory() {
            return self.InnerGetDirectory(
                FolderStructure.BackupDirectoryName
            );
        }

        public IDirectory GetDatabaseDirectory() {
            return self.InnerGetDirectory(
                FolderStructure.DatabaseDirectoryName
            );
        }

        public IDirectory GetTemporaryDirectory() {
            return self.InnerGetDirectory(
                FolderStructure.TemporaryDirectoryName
            );
        }

        public IDirectory GetTemporaryDirectory(DateTimeOffset timestamp) {
            return self.InnerGetDirectory(
                Path.Combine(
                    FolderStructure.TemporaryDirectoryName,
                    $"{timestamp:yyyyMMddHHmmss}"
                )
            );
        }

        public IDirectory GetUpdateDirectory() {
            return self.InnerGetDirectory(
                FolderStructure.UpdateDirectoryName
            );
        }

        private IDirectory InnerGetDirectory(string relativePath) {
            return self.GetDirectory(relativePath);
        }
    }
}