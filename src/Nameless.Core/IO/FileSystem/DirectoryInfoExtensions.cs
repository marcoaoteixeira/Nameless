using Nameless.Helpers;

namespace Nameless.IO.FileSystem;

internal static class DirectoryInfoExtensions {
    internal static string GetFullPath(this DirectoryInfo self) {
        return PathHelper.Normalize(self.FullName);
    }
}