using Nameless.IO;

namespace Nameless.Testing.Tools.IO;

/// <summary>
///     Unique, self-cleaning directory under the system temporary folder.
/// </summary>
public sealed class TempDirectory : IDisposable {
    /// <summary>
    ///     Gets the temp directory path.
    /// </summary>
    public string Path { get; }

    /// <summary>
    ///     Initializes a new instance of <see cref="TempDirectory"/> class.
    /// </summary>
    /// <param name="identifier">
    ///     The directory identifier, if not provided "Nameless" value will
    ///     be used.
    /// </param>
    public TempDirectory(string? identifier = null) {
        identifier = string.IsNullOrWhiteSpace(identifier) ? "Nameless" : PathHelper.Sanitize(identifier);

        Path = SysPath.Combine(SysPath.GetTempPath(), identifier, $"{Guid.CreateVersion7():N}");

        SysDirectory.CreateDirectory(Path);
    }

    /// <summary>
    ///     Combines other path parts.
    /// </summary>
    /// <param name="parts">
    ///     A collection of path parts.
    /// </param>
    /// <returns>
    ///     The combined path.
    /// </returns>
    public string Combine(params string[] parts) {
        return SysPath.Combine([Path, .. parts]);
    }

    /// <summary>
    ///     Creates a file inside the temp directory.
    /// </summary>
    /// <param name="relativePath">
    ///     The file relative path.
    /// </param>
    /// <param name="content">
    ///     When provided, use it for the file content.
    /// </param>
    /// <returns>
    ///     The path to the file.
    /// </returns>
    public string CreateFile(string relativePath, string? content) {
        return InnerCreateFile(
            relativePath,
            path => SysFile.WriteAllText(path, content ?? string.Empty)
        );
    }

    /// <summary>
    ///     Creates a file inside the temp directory.
    /// </summary>
    /// <param name="relativePath">
    ///     The file relative path.
    /// </param>
    /// <param name="content">
    ///     When provided, use it for the file content.
    /// </param>
    /// <returns>
    ///     The path to the file.
    /// </returns>
    public string CreateFile(string relativePath, byte[] content) {
        return InnerCreateFile(
            relativePath,
            path => SysFile.WriteAllBytes(path, content)
        );
    }

    /// <summary>
    ///     Creates a subdirectory inside the temp directory.
    /// </summary>
    /// <param name="relativePath">
    ///     The directory relative path.
    /// </param>
    /// <returns>
    ///     The path to the directory.
    /// </returns>
    public string CreateDirectory(string relativePath) {
        var path = Combine(relativePath);

        return SysDirectory.CreateDirectory(path).FullName;
    }

    /// <inheritdoc />
    public void Dispose() {
        try { SysDirectory.Delete(Path, recursive: true); }
        catch (IOException) { /* best effort */ }
        catch (UnauthorizedAccessException) { /* best effort */ }
    }

    private string InnerCreateFile(string relativePath, Action<string> write) {
        var path = Combine(relativePath);

        var directoryPath = SysPath.GetDirectoryName(path) ?? throw new InvalidOperationException(
            $"Can't extract directory path from relative path: {relativePath}"
        );

        SysDirectory.CreateDirectory(directoryPath);

        write(path);

        return path;
    }
}
