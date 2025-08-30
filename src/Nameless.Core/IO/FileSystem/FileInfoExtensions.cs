using Nameless.Helpers;

namespace Nameless.IO.FileSystem;

internal static class FileInfoExtensions {
    internal static string GetFullPath(this FileInfo self) {
        return PathHelper.Normalize(self.FullName);
    }
}