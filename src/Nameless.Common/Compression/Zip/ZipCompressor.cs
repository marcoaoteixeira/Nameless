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
        var validation = ValidateCompressRequest(request).ToArray();
        if (validation.Length > 0) { return validation; }

        try
        {
            SysDirectory.CreateDirectory(
                SysPath.GetDirectoryName(request.DestinationFilePath)!
            );

            await using var zip = await ZipFile.OpenAsync(
                archiveFileName: request.DestinationFilePath,
                mode: ZipArchiveMode.Create,
                cancellationToken: cancellationToken
            );

            foreach (var file in request.Files) {
                var fileName = SysPath.Combine(
                    file.DirectoryPath ?? string.Empty,
                    SysPath.GetFileName(file.Path)
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
            CommonLog.Error(_logger, ex.Message, ex, GetType().Tag);

            return Error.Failure(ex.Message, ex: ex);
        }

        return new CompressMetadata(request.DestinationFilePath);
    }

    /// <inheritdoc />
    public async Task<DecompressResponse> DecompressAsync(DecompressRequest request, CancellationToken cancellationToken) {
        var validation = ValidateDecompressRequest(request).ToArray();
        if (validation.Length > 0) { return validation; }

        DirectoryInfo destinationDirectory;

        try {
            if (string.IsNullOrWhiteSpace(request.DestinationDirectoryPath)) {
                var destinationDirectoryPath = SysPath.GetDirectoryName(request.SourceFilePath) ??
                                               SysPath.GetPathRoot(request.SourceFilePath) ??
                                               SysPath.GetTempPath();

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
            CommonLog.Error(_logger, ex.Message, ex, GetType().Tag);

            return Error.Failure(ex.Message, ex: ex);
        }

        return new DecompressMetadata(destinationDirectory.FullName);
    }

    private static IEnumerable<Error> ValidateCompressRequest(CompressRequest request) {
        if (string.IsNullOrWhiteSpace(request.DestinationFilePath)) {
            yield return Error.Failure("Missing destination file path.");
        }

        if (SysFile.Exists(request.DestinationFilePath)) {
            yield return Error.Failure("Destination file already exists.");
        }

        var destinationDirectoryPath = SysPath.GetDirectoryName(request.DestinationFilePath);
        if (string.IsNullOrWhiteSpace(destinationDirectoryPath)) {
            yield return Error.Failure("Can't resolve destination directory path.");
        }

        if (!request.Files.Any()) {
            yield return Error.Failure("No files to compress.");
        }
    }

    private static IEnumerable<Error> ValidateDecompressRequest(DecompressRequest request) {
        if (string.IsNullOrWhiteSpace(request.SourceFilePath)) {
            yield return Error.Failure("Missing source file path.");
        }

        if (!SysFile.Exists(request.SourceFilePath)) {
            yield return Error.Failure("Source file does not exist.");
        }
    }
}