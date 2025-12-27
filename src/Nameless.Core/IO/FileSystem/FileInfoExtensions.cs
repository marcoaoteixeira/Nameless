using Nameless.Helpers;

namespace Nameless.IO.FileSystem;

internal static class FileInfoExtensions {
    extension(FileInfo self) {
        internal string GetFullPath() {
            return PathHelper.Normalize(self.FullName);
        }
    }
}