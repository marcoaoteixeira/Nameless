using Nameless.Testing.Tools.IO;

namespace Nameless.Compression.Infrastructure;

internal static class TempDirectoryExtensions {
    extension(TempDirectory self) {
        public string CreateArchive(params (string Name, string Content)[] entries) {
            return self.CreateFile($"archive-{Guid.CreateVersion7():N}.zip", ZipInspector.CreateArchive(entries));
        }

        public string CreateBinaryArchive(params (string Name, byte[] Content)[] entries) {
            return self.CreateFile($"archive-{Guid.CreateVersion7():N}.zip", ZipInspector.CreateBinaryArchive(entries));
        }
    }
}
