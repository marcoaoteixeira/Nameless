namespace Nameless.IO;

/// <summary>
/// Default implementation of <see cref="IFileSystem"/>
/// </summary>
public sealed class FileSystem : IFileSystem {
    /// <inheritdoc />
    public IDirectoryService Directory { get; } = new DirectoryService();

    /// <inheritdoc />
    public IFileService File { get; } = new FileService();

    /// <inheritdoc />
    public IPathService Path { get; } = new PathService();
}