namespace Nameless.Compression.Responses;

public readonly record struct DecompressArchiveMetadata(string DirectoryPath) {
    public bool IsDirectoryAvailable => Directory.Exists(DirectoryPath);
}