using System.Diagnostics;
using Microsoft.Extensions.FileProviders;
using Nameless.IO.Monitoring;

namespace Nameless.IO.Embedded;

/// <summary>
///     Read-only implementation of <see cref="IFile"/> for a file
///     embedded as resource into an assembly.
/// </summary>
[DebuggerDisplay(value: "{Path,nq}")]
public class EmbeddedFile : IFile {
    private readonly string _relativePath;
    private readonly EmbeddedFileProvider _provider;

    private IFileInfo FileInfo => _provider.Manifest.GetFileInfo(_relativePath);

    /// <inheritdoc />
    public string Name => EmbeddedPathUtils.GetName(_relativePath);

    /// <inheritdoc />
    /// <remarks>
    ///     Format: <c>embedded://{AssemblyName}/{RelativePath}</c>.
    /// </remarks>
    public string Path => $"{_provider.Root}{_relativePath}";

    /// <inheritdoc />
    public bool Exists => FileInfo is { Exists: true, IsDirectory: false };

    /// <inheritdoc />
    /// <remarks>
    ///     Returns the creation time of the assembly file, or
    ///     <see cref="DateTime.MinValue"/> when the assembly has
    ///     no location on disk.
    /// </remarks>
    public DateTime LastWriteTime => _provider.LastWriteTime;

    internal EmbeddedFile(string relativePath, EmbeddedFileProvider provider) {
        _relativePath = relativePath;
        _provider = provider;
    }

    /// <inheritdoc />
    /// <remarks>
    ///     <paramref name="mode"/>, <paramref name="access"/> and
    ///     <paramref name="share"/> are ignored since they do not apply
    ///     to embedded resources. The returned stream is read-only.
    /// </remarks>
    /// <exception cref="FileNotFoundException">
    ///     if the file is not embedded into the assembly.
    /// </exception>
    public Stream Open(FileMode mode, FileAccess access, FileShare share) {
        var fileInfo = FileInfo;

        if (fileInfo is not { Exists: true, IsDirectory: false }) {
            throw new FileNotFoundException("Embedded file not found.", Path);
        }

        return fileInfo.CreateReadStream();
    }

    /// <inheritdoc />
    /// <exception cref="InvalidOperationException">
    ///     always, embedded files cannot be deleted.
    /// </exception>
    public void Delete() {
        throw new InvalidOperationException("Embedded files cannot be deleted.");
    }

    /// <inheritdoc />
    /// <exception cref="InvalidOperationException">
    ///     always, embedded files cannot be copied into the assembly.
    /// </exception>
    public IFile Copy(string destinationRelativePath, bool overwrite) {
        throw new InvalidOperationException("Embedded files cannot be copied.");
    }

    /// <inheritdoc />
    /// <exception cref="InvalidOperationException">
    ///     always, embedded files cannot be monitored.
    /// </exception>
    public IFileMonitor Monitor() {
        throw new InvalidOperationException("Embedded files cannot be monitored.");
    }
}
