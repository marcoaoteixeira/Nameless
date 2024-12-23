namespace Nameless.FileSystem;

/// <summary>
/// Default implementation of <see cref="IFileSystem"/>
/// </summary>
public sealed class FileSystem : IFileSystem {
    private readonly DirectoryService _directoryService = new();
    /// <inheritdoc />
    public IDirectoryService Directory => _directoryService;

    private readonly FileService _fileService = new();
    /// <inheritdoc />
    public IFileService File => _fileService;

    private readonly PathService _pathService = new();
    /// <inheritdoc />
    public IPathService Path => _pathService;
}