using System.IO.Compression;
using Microsoft.Extensions.Logging;
using Nameless.Compression.Requests;
using Nameless.Compression.Responses;
using Nameless.ObjectModel;

namespace Nameless.Compression.Zip;

/// <summary>
///     ZIP implementation of <see cref="ICompressor"/>.
/// </summary>
public class ZipCompressor : ICompressor {
    private readonly ILogger<ZipCompressor> _logger;

    /// <summary>
    ///     Initializes a new instance of <see cref="ZipCompressor"/> class.
    /// </summary>
    /// <param name="logger">
    ///     The logger.
    /// </param>
    public ZipCompressor(ILogger<ZipCompressor> logger) {
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<CompressResponse> CompressAsync(CompressRequest request, CancellationToken cancellationToken) {
        var validation = ValidateCompressRequest(request);
        if (validation.Length > 0) { return validation; }

        try
        {
            Directory.CreateDirectory(
                Path.GetDirectoryName(request.DestinationFilePath)!
            );

            await using var zip = await ZipFile.OpenAsync(
                archiveFileName: request.DestinationFilePath,
                mode: ZipArchiveMode.Create,
                cancellationToken: cancellationToken
            );

            foreach (var file in request.Files) {
                var fileName = Path.Combine(
                    file.DirectoryPath ?? string.Empty,
                    Path.GetFileName(file.Path)
                );

                await zip.CreateEntryFromFileAsync(
                    sourceFileName: file.Path,
                    entryName: fileName,
                    compressionLevel: request.CompressionLevel,
                    cancellationToken: cancellationToken
                ).SkipContextSync();
            }
        }
        catch (Exception ex) {
            Log.CompressAsyncFailure(_logger, ex);

            return Error.Failure(ex.Message);
        }

        return new CompressMetadata(request.DestinationFilePath);
    }

    /// <inheritdoc />
    public async Task<DecompressResponse> DecompressAsync(DecompressRequest request, CancellationToken cancellationToken) {
        var validation = ValidateDecompressRequest(request);
        if (validation.Length > 0) { return validation; }

        DirectoryInfo destinationDirectory;

        try {
            if (string.IsNullOrWhiteSpace(request.DestinationDirectoryPath)) {
                var destinationDirectoryPath = Path.GetDirectoryName(request.SourceFilePath) ??
                                               Path.GetPathRoot(request.SourceFilePath) ??
                                               Path.GetTempPath();

                destinationDirectory = new DirectoryInfo(destinationDirectoryPath);
            }
            else { destinationDirectory = new DirectoryInfo(request.DestinationDirectoryPath); }

            // Ensure destination directory existence
            destinationDirectory.Create();

            await ZipFile.ExtractToDirectoryAsync(
                request.SourceFilePath,
                destinationDirectory.FullName,
                cancellationToken
            );

        }
        catch (Exception ex) {
            Log.DecompressAsyncFailure(_logger, ex);

            return Error.Failure(ex.Message);
        }

        return new DecompressMetadata(destinationDirectory.FullName);
    }

    private static Error[] ValidateCompressRequest(CompressRequest request) {
        var result = new List<Error>();

        if (string.IsNullOrWhiteSpace(request.DestinationFilePath)) {
            result.Add(Error.Failure("Missing destination file path."));
        }

        if (File.Exists(request.DestinationFilePath)) {
            result.Add(Error.Failure("Destination file already exists."));
        }

        var destinationDirectoryPath = Path.GetDirectoryName(request.DestinationFilePath);
        if (string.IsNullOrWhiteSpace(destinationDirectoryPath)) {
            result.Add(Error.Failure("Can't resolve destination directory path."));
        }

        if (!request.Files.Any()) {
            result.Add(Error.Failure("No files to compress."));
        }

        return [.. result];
    }

    private static Error[] ValidateDecompressRequest(DecompressRequest request) {
        var result = new List<Error>();

        if (string.IsNullOrWhiteSpace(request.SourceFilePath)) {
            result.Add(Error.Failure("Missing source file path."));
        }

        if (!File.Exists(request.SourceFilePath)) {
            result.Add(Error.Failure("Source file does not exist."));
        }

        return [.. result];
    }
}