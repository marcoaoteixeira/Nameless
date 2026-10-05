using System.IO.Compression;
using System.Text;

namespace Nameless.Compression.Infrastructure;

/// <summary>
/// Reads and builds ZIP archives with the plain BCL, independently of the system under test.
/// </summary>
internal static class ZipInspector {
    /// <summary>
    /// Entry name → text content (<see langword="null"/> for directory entries).
    /// </summary>
    public static async Task<IReadOnlyDictionary<string, string?>> ReadEntriesAsync(string zipPath) {
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var archive = await ZipFile.OpenReadAsync(zipPath, cancellationToken);
        var entries = new Dictionary<string, string?>();

        foreach (var entry in archive.Entries) {
            entries.Add(entry.FullName, entry.FullName.EndsWith('/') ? null : await ReadTextAsync(entry, cancellationToken));
        }

        return entries;
    }

    /// <summary>
    /// Entry name <c>CompressFilesAsync</c> stores for a file: its full path minus the root, with '/' separators.
    /// </summary>
    public static string EntryFor(string fullPath)
        => SysPath.GetRelativePath(SysPath.GetPathRoot(fullPath)!, fullPath).Replace(SysPath.DirectorySeparatorChar, '/');

    /// <summary>
    /// Where a file compressed with <c>CompressFilesAsync</c> lands after extraction into <paramref name="destination"/>.
    /// </summary>
    public static string ExtractedPath(string destination, string fullPath)
        => SysPath.Combine(destination, SysPath.GetRelativePath(SysPath.GetPathRoot(fullPath)!, fullPath));

    public static async Task<ZipArchiveEntry[]> ReadEntryMetadataAsync(string zipPath) {
        await using var archive = await ZipFile.OpenReadAsync(zipPath, TestContext.Current.CancellationToken);

        return [.. archive.Entries];
    }

    public static byte[] CreateArchive(params (string Name, string Content)[] entries) {
        using var buffer = new MemoryStream();

        using (var archive = new ZipArchive(buffer, ZipArchiveMode.Create, leaveOpen: true)) {
            foreach (var (name, content) in entries) {
                var entry = archive.CreateEntry(name);

                if (name.EndsWith('/')) { continue; }

                using var writer = new StreamWriter(entry.Open(), Encoding.UTF8);
                writer.Write(content);
            }
        }

        return buffer.ToArray();
    }

    public static byte[] CreateBinaryArchive(params (string Name, byte[] Content)[] entries) {
        using var buffer = new MemoryStream();

        using (var archive = new ZipArchive(buffer, ZipArchiveMode.Create, leaveOpen: true)) {
            foreach (var (name, content) in entries) {
                using var entryStream = archive.CreateEntry(name, CompressionLevel.SmallestSize).Open();
                entryStream.Write(content);
            }
        }

        return buffer.ToArray();
    }

    /// <summary>
    /// Rewrites the declared uncompressed size of every entry (local + central headers), simulating a lying archive.
    /// </summary>
    public static byte[] WithDeclaredUncompressedSize(byte[] archive, uint declaredSize)
        => PatchHeaders(archive, localOffset: 22, centralOffset: 24, declaredSize);

    /// <summary>
    /// Rewrites the declared CRC-32 of every entry (local + central headers), simulating a corrupted entry.
    /// </summary>
    public static byte[] WithDeclaredCrc32(byte[] archive, uint crc32)
        => PatchHeaders(archive, localOffset: 14, centralOffset: 16, crc32);

    private static byte[] PatchHeaders(byte[] archive, int localOffset, int centralOffset, uint value) {
        const uint LocalHeaderSignature = 0x04034b50;
        const uint CentralHeaderSignature = 0x02014b50;

        var patched = (byte[])archive.Clone();

        for (var offset = 0; offset <= patched.Length - 4; offset++) {
            var fieldOffset = BitConverter.ToUInt32(patched, offset) switch {
                LocalHeaderSignature => offset + localOffset,
                CentralHeaderSignature => offset + centralOffset,
                _ => -1,
            };

            if (fieldOffset >= 0) {
                BitConverter.TryWriteBytes(patched.AsSpan(fieldOffset, 4), value);
            }
        }

        return patched;
    }

    private static async Task<string> ReadTextAsync(ZipArchiveEntry entry, CancellationToken cancellationToken) {
        using var reader = new StreamReader(await entry.OpenAsync(cancellationToken), Encoding.UTF8);

        return await reader.ReadToEndAsync(cancellationToken);
    }
}
