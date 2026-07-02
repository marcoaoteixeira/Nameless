namespace Nameless.IO;

internal static class DirectoryInfoExtensions {
    extension(DirectoryInfo self) {
        internal string GetFullPath() {
            return PathHelper.Normalize(self.FullName);
        }
    }
}