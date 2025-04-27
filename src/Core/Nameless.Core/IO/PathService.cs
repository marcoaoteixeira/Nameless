namespace Nameless.IO;

/// <summary>
/// Default implementation of <see cref="IPathService"/>
/// </summary>
public sealed class PathService : IPathService {
    /// <inheritdoc />
    public string GetTempFileName()
        => Path.GetTempFileName();

    /// <inheritdoc />
    public string GetExtension(string filePath)
        => Path.GetExtension(filePath);

    /// <inheritdoc />
    public string GetFileName(string filePath)
        => Path.GetFileName(filePath);

    /// <inheritdoc />
    public string GetFileNameWithoutExtension(string filePath)
        => Path.GetFileNameWithoutExtension(filePath);

    /// <inheritdoc />
    public string Combine(params string[] parts)
        => Path.Combine(parts);
}