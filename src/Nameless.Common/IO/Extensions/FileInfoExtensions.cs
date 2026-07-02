namespace Nameless.IO;

internal static class FileInfoExtensions {
    extension(FileInfo self) {
        internal string GetFullPath() {
            return PathHelper.Normalize(self.FullName);
        }
    }
}