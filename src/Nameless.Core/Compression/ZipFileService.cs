using System.Diagnostics;
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
public class ZipFileService : IZipFileService {
    private readonly ILogger<ZipFileService> _logger;

    /// <summary>
    ///     Initializes a new instance of <see cref="ZipFileService"/>
    ///     class.
    /// </summary>
    /// <param name="logger">
    ///     The logger.
    /// </param>
    public ZipFileService(ILogger<ZipFileService> logger) {
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<CompressResponse> CompressAsync(CompressRequest request, CancellationToken cancellationToken) {
        var validation = ValidateCompressRequest(request);
        if (validation.Length > 0) {
            return validation;
        }

        var destinationDirectoryPath = Path.GetDirectoryName(request.DestinationFilePath);
        if (string.IsNullOrWhiteSpace(destinationDirectoryPath)) {
            return Error.Failure("Can't resolve destination directory path.");
        }

        Directory.CreateDirectory(destinationDirectoryPath);

        var sw = Stopwatch.StartNew();

        try {
            _logger.CompressStarting();

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
            _logger.CompressFailure(ex);

            return Error.Failure(ex.Message);
        }
        finally { _logger.CompressFinished(sw.ElapsedMilliseconds); }

        return new CompressMetadata(request.DestinationFilePath);
    }

    /// <inheritdoc/>
    public async Task<DecompressResponse> DecompressAsync(DecompressRequest request, CancellationToken cancellationToken) {
        var validation = ValidateDecompressRequest(request);
        if (validation.Length > 0) {
            return validation;
        }

        // Ensure destination directory exists
        var destinationDirectoryPath = GetDestinationDirectoryPath(request);

        var sw = Stopwatch.StartNew();

        try {
            _logger.DecompressStarting();

            await ZipFile.ExtractToDirectoryAsync(
                request.SourceFilePath,
                destinationDirectoryPath,
                cancellationToken
            );
        }
        catch (Exception ex) {
            _logger.DecompressFailure(ex);

            return Error.Failure(ex.Message);
        }
        finally { _logger.DecompressFinished(sw.ElapsedMilliseconds); }

        return new DecompressMetadata(destinationDirectoryPath);
    }

    private static Error[] ValidateCompressRequest(CompressRequest request) {
        var result = new List<Error>();

        if (string.IsNullOrWhiteSpace(request.DestinationFilePath)) {
            result.Add(Error.Failure("Missing destination file path."));
        }

        if (File.Exists(request.DestinationFilePath)) {
            result.Add(Error.Failure("Destination file already exists."));
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

    private static string GetDestinationDirectoryPath(DecompressRequest request) {
        if (!string.IsNullOrWhiteSpace(request.DestinationDirectoryPath)) {
            return Directory.CreateDirectory(request.DestinationDirectoryPath).FullName;
        }
        
        var result = Path.Combine(
            Path.GetTempPath(),
            Path.GetFileNameWithoutExtension(request.SourceFilePath)
        );

        return Directory.CreateDirectory(result).FullName;
    }
}