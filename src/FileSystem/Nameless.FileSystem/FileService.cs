namespace Nameless.FileSystem;

public sealed class FileService : IFileService {
    /// <inheritdoc />
    public void Copy(string sourceFilePath, string destinationFilePath, bool overwrite)
        => File.Copy(sourceFilePath, destinationFilePath, overwrite);

    /// <inheritdoc />
    public Stream Open(string filePath, FileMode mode, FileAccess access, FileShare share)
        => File.Open(filePath, mode, access, share);

    /// <inheritdoc />
    public Stream Create(string filePath)
        => File.Create(filePath);

    /// <inheritdoc />
    public void Delete(string filePath)
        => File.Delete(filePath);

    /// <inheritdoc />
    public bool Exists(string filePath)
        => File.Exists(filePath);

    /// <inheritdoc />
    public string ReadAllText(string filePath)
        => File.ReadAllText(filePath);

    /// <inheritdoc />
    public byte[] ReadAllBytes(string filePath)
        => File.ReadAllBytes(filePath);
}