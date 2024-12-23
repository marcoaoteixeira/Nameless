namespace Nameless.FileSystem;

public sealed class DirectoryService : IDirectoryService {
    /// <inheritdoc />
    public IFile[] GetFiles(string directoryPath, string filter, bool recursive)
        => Directory.GetFiles(directoryPath, filter, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)
                    .Select(file => new FileWrapper(new FileInfo(file)))
                    .ToArray<IFile>();

    /// <inheritdoc />
    public IDirectory Create(string directoryPath)
        => new DirectoryWrapper(Directory.CreateDirectory(directoryPath));

    /// <inheritdoc />
    public bool Exists(string directoryPath)
        => Directory.Exists(directoryPath);

    /// <inheritdoc />
    public void Delete(string directoryPath, bool recursive)
        => Directory.Delete(directoryPath, recursive);

    public IDirectory[] GetDirectories(string rootPath, string filter, bool recursive)
        => Directory.GetDirectories(rootPath,
                                    filter,
                                    recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)
                    .Select(path => new DirectoryWrapper(new DirectoryInfo(path)))
                    .ToArray<IDirectory>();
}