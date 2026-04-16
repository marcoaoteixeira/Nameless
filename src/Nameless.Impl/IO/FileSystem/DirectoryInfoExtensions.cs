using Nameless.Helpers;

namespace Nameless.IO.FileSystem;

internal static class DirectoryInfoExtensions {
    extension(DirectoryInfo self) {
        internal string GetFullPath() {
            return PathHelper.Normalize(self.FullName);
        }
    }
}