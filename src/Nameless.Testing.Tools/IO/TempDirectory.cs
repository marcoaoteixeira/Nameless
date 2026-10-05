using Nameless.IO;

namespace Nameless.Testing.Tools.IO;

/// <summary>
/// Unique, self-cleaning directory under the system temporary folder.
/// </summary>
public sealed class TempDirectory : IDisposable {
    public string Path { get; }

    public TempDirectory(string? identifier = null) {
        identifier = string.IsNullOrWhiteSpace(identifier)
            ? "Nameless"
            : PathHelper.Sanitize(identifier);

        Path = SysPath.Combine(SysPath.GetTempPath(), identifier, $"{Guid.CreateVersion7():N}");

        SysDirectory.CreateDirectory(Path);
    }

    public string Combine(params string[] parts) {
        return SysPath.Combine([Path, .. parts]);
    }

    public string CreateFile(string relativePath, string? content = null) {
        return InnerCreateFile(
            relativePath,
            path => SysFile.WriteAllText(path, content ?? string.Empty)
        );
    }

    public string CreateFile(string relativePath, byte[] content) {
        return InnerCreateFile(
            relativePath,
            path => SysFile.WriteAllBytes(path, content)
        );
    }

    public string CreateDirectory(string relativePath) {
        var path = Combine(relativePath);

        return SysDirectory.CreateDirectory(path).FullName;
    }

    public void Dispose() {
        try { SysDirectory.Delete(Path, recursive: true); }
        catch (IOException) { /* best effort */ }
        catch (UnauthorizedAccessException) { /* best effort */ }
    }

    private string InnerCreateFile(string relativePath, Action<string> write) {
        var path = Combine(relativePath);

        var directoryPath = SysPath.GetDirectoryName(path)
            ?? throw new InvalidOperationException($"Can't extract directory path from relative path: {relativePath}");

        SysDirectory.CreateDirectory(directoryPath);

        write(path);

        return path;
    }
}
