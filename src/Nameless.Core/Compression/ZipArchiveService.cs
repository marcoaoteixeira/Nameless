using System.IO.Compression;
using Microsoft.Extensions.Logging;
using Nameless.Compression.Requests;
using Nameless.Compression.Responses;
using Nameless.ObjectModel;

namespace Nameless.Compression;

/// <summary>
///     Provides methods for compressing files into ZIP archives and
///     extracting files from ZIP archives asynchronously.
/// </summary>
/// <remarks>
///     This service implements the IZipArchiveService interface and
///     supports asynchronous operations for creating and extracting
///     ZIP archives. It validates input requests and ensures that
///     destination directories exist before performing archive
///     operations. All methods are designed to be used with cancellation
///     tokens for responsive cancellation support.
/// </remarks>
public class ZipArchiveService : IZipArchiveService {
    private readonly ILogger<ZipArchiveService> _logger;

    public ZipArchiveService(ILogger<ZipArchiveService> logger) {
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<CompressArchiveResponse> CompressAsync(CompressArchiveRequest request, CancellationToken cancellationToken) {
        var validation = ValidateCompressArchiveRequest(request);
        if (validation.Length > 0) {
            return validation;
        }

        var destinationDirectoryPath = Path.GetDirectoryName(request.DestinationFilePath);
        if (string.IsNullOrWhiteSpace(destinationDirectoryPath)) {
            return Error.Failure("Can't resolve destination directory path.");
        }

        Directory.CreateDirectory(destinationDirectoryPath);

        try {
            await using var zipArchive = await ZipFile.OpenAsync(
                archiveFileName: request.DestinationFilePath,
                mode: ZipArchiveMode.Create,
                cancellationToken: cancellationToken
            );

            foreach (var file in request.Files) {
                var fileName = Path.Combine(
                    file.DirectoryPath ?? string.Empty,
                    Path.GetFileName(file.Path)
                );

                await zipArchive.CreateEntryFromFileAsync(
                    sourceFileName: file.Path,
                    entryName: fileName,
                    compressionLevel: request.CompressionLevel,
                    cancellationToken: cancellationToken
                ).SkipContextSync();
            }
        }
        catch (Exception ex) {
            _logger.CompressArchiveFailure(ex);

            return Error.Failure(ex.Message);
        }

        return new CompressArchiveMetadata(request.DestinationFilePath);
    }

    /// <inheritdoc/>
    public async Task<DecompressArchiveResponse> DecompressAsync(DecompressArchiveRequest request, CancellationToken cancellationToken) {
        var validation = ValidateDecompressArchiveRequest(request);
        if (validation.Length > 0) {
            return validation;
        }

        // Ensure destination directory exists
        Directory.CreateDirectory(request.DestinationDirectoryPath);

        try {
            await ZipFile.ExtractToDirectoryAsync(
                request.SourceFilePath,
                request.DestinationDirectoryPath,
                cancellationToken
            );
        }
        catch (Exception ex) {
            _logger.DecompressArchiveFailure(ex);

            return Error.Failure(ex.Message);
        }

        return new DecompressArchiveMetadata(request.DestinationDirectoryPath);
    }

    private static Error[] ValidateCompressArchiveRequest(CompressArchiveRequest request) {
        var result = new List<Error>();

        if (string.IsNullOrWhiteSpace(request.DestinationFilePath)) {
            result.Add(Error.Failure("Missing destination file path."));
        }

        if (File.Exists(request.DestinationFilePath)) {
            result.Add(Error.Failure("Destination file already exists."));
        }

        if (!request.Files.Any()) {
            result.Add(Error.Failure("No file or directory to compress."));
        }

        return [.. result];
    }

    private static Error[] ValidateDecompressArchiveRequest(DecompressArchiveRequest request) {
        var result = new List<Error>();

        if (string.IsNullOrWhiteSpace(request.SourceFilePath)) {
            result.Add(Error.Failure("Missing source file path."));
        }

        if (!File.Exists(request.SourceFilePath)) {
            result.Add(Error.Failure("Source file do not exists."));
        }

        if (string.IsNullOrWhiteSpace(request.DestinationDirectoryPath)) {
            result.Add(Error.Failure("Missing destination directory path."));
        }

        return [.. result];
    }
}