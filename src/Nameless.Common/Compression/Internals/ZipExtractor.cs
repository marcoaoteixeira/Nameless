using System.Buffers;
using System.Diagnostics;
using System.IO.Compression;
using Microsoft.Extensions.Logging;

namespace Nameless.Compression.Internals;

/// <summary>
///     Extracts ZIP archives defensively.
/// </summary>
/// <remarks>
///     Extraction runs in two phases. Planning inspects every entry before
///     anything is written and rejects: too many entries, declared sizes
///     or ratios beyond <see cref="ZipCompressorOptions"/>, entries that would
///     land outside the destination (other drives, UNC paths, rooted names,
///     <c>..</c> traversal, NTFS alternate data streams), Windows reserved
///     device names (on Windows only), and existing files when overwriting
///     is not allowed. Writing then streams each entry while counting the
///     bytes actually produced and verifying size and CRC-32, because the
///     sizes an archive declares about itself cannot be trusted. Failures
///     while writing (disk full, locked files, limit violations) propagate
///     as-is; only the file being written at that moment is removed.
/// </remarks>
internal sealed class ZipExtractor(ZipCompressorOptions options, ILogger logger) {
    private const int BUFFER_SIZE = 81_920;

    public async Task ExtractAsync(Stream source, string destination, bool overwrite, CancellationToken cancellationToken) {
        var sw = Stopwatch.StartNew();

        try {
            cancellationToken.ThrowIfCancellationRequested();

            await using var archive = await ZipArchive.CreateAsync(source, ZipArchiveMode.Read, leaveOpen: true, entryNameEncoding: null, cancellationToken)
                                                      .ConfigureAwait(false);

            var plan = Plan(archive, destination, overwrite);

            SysDirectory.CreateDirectory(destination);

            long totalWritten = 0;

            foreach (var (entry, target) in plan) {
                cancellationToken.ThrowIfCancellationRequested();
                Log.EntryExtracted(logger, entry.FullName, target);

                if (IsDirectory(entry)) {
                    SysDirectory.CreateDirectory(target);
                    continue;
                }

                totalWritten = await ExtractFileAsync(entry, target, overwrite, totalWritten, cancellationToken)
                    .ConfigureAwait(false);
            }

            Log.ExtractionCompleted(logger, plan.Count, totalWritten, destination, sw.ElapsedMilliseconds);
        }
        catch (Exception ex) {
            Log.ExtractionFailed(logger, ex, destination);

            throw;
        }
    }

    private List<(ZipArchiveEntry Entry, string Target)> Plan(ZipArchive archive, string destination, bool overwrite) {
        var entries = archive.Entries;

        if (entries.Count > options.Limits.MaxEntryCount) {
            throw Rejected($"The archive contains {entries.Count} entries, above the limit of {options.Limits.MaxEntryCount}.");
        }

        var root = SysPath.TrimEndingDirectorySeparator(destination);
        var plan = new List<(ZipArchiveEntry, string)>(entries.Count);
        long declaredTotal = 0;

        foreach (var entry in entries) {
            var target = ResolveTarget(root, entry.FullName);

            if (!IsDirectory(entry)) {
                declaredTotal += entry.Length;
                EnsureTotalWithinLimit(declaredTotal);
                EnsureRatioWithinLimit(entry, entry.Length);

                if (!overwrite && SysFile.Exists(target)) {
                    throw new IOException($"The file '{target}' already exists.");
                }
            }

            plan.Add((entry, target));
        }

        Log.ExtractionPlanned(logger, destination, plan.Count, declaredTotal);

        return plan;
    }

    private async Task<long> ExtractFileAsync(ZipArchiveEntry entry, string target, bool overwrite, long totalWritten, CancellationToken cancellationToken) {
        SysDirectory.CreateDirectory(SysPath.GetDirectoryName(target)!);

        var buffer = ArrayPool<byte>.Shared.Rent(BUFFER_SIZE);
        var created = false;

        try {
            await using var input = await entry.OpenAsync(cancellationToken).ConfigureAwait(false);
            await using var output = new FileStream(target, overwrite ? FileMode.Create : FileMode.CreateNew, FileAccess.Write, FileShare.None, BUFFER_SIZE, FileOptions.Asynchronous);

            created = true;

            long written = 0;
            var crc = Crc32.Initial;
            int read;

            while ((read = await input.ReadAsync(buffer, cancellationToken).ConfigureAwait(false)) > 0) {
                written += read;
                totalWritten += read;

                if (written > entry.Length) {
                    throw Rejected($"Entry '{entry.FullName}' is larger than its declared size of {entry.Length} bytes.");
                }

                EnsureTotalWithinLimit(totalWritten);
                EnsureRatioWithinLimit(entry, written);

                crc = Crc32.Update(crc, buffer.AsSpan(0, read));

                await output.WriteAsync(buffer.AsMemory(0, read), cancellationToken).ConfigureAwait(false);
            }

            // The BCL stops reading at the declared size without complaint
            // and skips CRC validation in that case, so a lying header would
            // otherwise yield a silently truncated file.
            if (written != entry.Length || Crc32.Finish(crc) != entry.Crc32) {
                throw Rejected($"Entry '{entry.FullName}' is corrupt: its content does not match the declared size and CRC-32.");
            }
        }
        catch when (created) {
            // The output stream is already disposed here; drop the
            // half-written file and let the failure propagate.
            TryDelete(target);

            throw;
        }
        finally { ArrayPool<byte>.Shared.Return(buffer); }

        TrySetLastWriteTime(target, entry.LastWriteTime);

        return totalWritten;
    }

    /// <exception cref="IOException">
    ///     The entry would be written outside <paramref name="root"/> or
    ///     uses a reserved name.
    /// </exception>
    private string ResolveTarget(string root, string entryName) {
        // ':' covers drive letters (C:\, C:relative) and NTFS alternate
        // data streams (file.txt:stream); leading separators cover rooted
        // and UNC names (/x, \x, \\server\share, //server/share).
        if (entryName.Contains(':') ||
            entryName.StartsWith(SysPath.DirectorySeparatorChar) ||
            entryName.StartsWith(SysPath.AltDirectorySeparatorChar)) {
            throw UnsafeEntry(entryName, root, "rooted, drive-qualified or alternate data stream name");
        }

        if (OperatingSystem.IsWindows() && WindowsReservedNames.ContainsReservedSegment(entryName)) {
            throw UnsafeEntry(entryName, root, "Windows reserved device name");
        }

        var target = SysPath.GetFullPath(SysPath.Combine(root, entryName));
        var relative = SysPath.GetRelativePath(root, target);

        if (SysPath.IsPathRooted(relative) || relative == ".." || relative.StartsWith($"..{SysPath.DirectorySeparatorChar}", StringComparison.Ordinal)) {
            throw UnsafeEntry(entryName, root, "path escapes the destination");
        }

        return target;
    }

    private void EnsureTotalWithinLimit(long total) {
        if (total > options.Limits.MaxTotalUncompressedBytes) {
            throw Rejected($"The archive expands beyond the limit of {options.Limits.MaxTotalUncompressedBytes} bytes.");
        }
    }

    private void EnsureRatioWithinLimit(ZipArchiveEntry entry, long uncompressedBytes) {
        if (uncompressedBytes > options.Limits.CompressionRatioThresholdBytes && uncompressedBytes > options.Limits.MaxCompressionRatio * Math.Max(entry.CompressedLength, 1)) {
            throw Rejected($"Entry '{entry.FullName}' expands beyond the {options.Limits.MaxCompressionRatio}:1 compression ratio limit.");
        }
    }

    private InvalidDataException Rejected(string reason) {
        Log.ArchiveRejected(logger, reason);

        return new InvalidDataException(reason);
    }

    private IOException UnsafeEntry(string entryName, string root, string reason) {
        Log.UnsafeEntryRejected(logger, entryName, reason);

        return new IOException(
            $"Entry '{entryName}' cannot be extracted into '{root}': {reason}."
        );
    }

    private static bool IsDirectory(ZipArchiveEntry entry) {
        return entry.FullName.EndsWith(SysPath.DirectorySeparatorChar) ||
               entry.FullName.EndsWith(SysPath.AltDirectorySeparatorChar);
    }

    private static void TrySetLastWriteTime(string path, DateTimeOffset lastWriteTime) {
        try { SysFile.SetLastWriteTime(path, lastWriteTime.DateTime); }
        catch (ArgumentOutOfRangeException) { /* timestamp not representable on this file system */ }
        catch (IOException) { /* best effort */ }
    }

    private static void TryDelete(string path) {
        try { SysFile.Delete(path); }
        catch (IOException) { /* best effort */ }
        catch (UnauthorizedAccessException) { /* best effort */ }
    }
}
