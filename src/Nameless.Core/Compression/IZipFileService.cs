using Nameless.Compression.Requests;
using Nameless.Compression.Responses;

namespace Nameless.Compression;

/// <summary>
///     Defines methods for compressing and decompressing data using
///     ZIP file formats asynchronously.
/// </summary>
public interface IZipFileService {
    /// <summary>
    ///     Asynchronously compresses the specified files and directories
    ///     into an archive according to the provided request parameters.
    /// </summary>
    /// <param name="request">
    ///     An object that specifies the files, directories, and compression
    ///     options to use for creating the archive.
    /// </param>
    /// <param name="cancellationToken">
    ///     A token that can be used to cancel the compression operation.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous compression operation.
    ///     The task result contains a <see cref="CompressResponse"/>
    ///     with details about the created archive.
    /// </returns>
    Task<CompressResponse> CompressAsync(CompressRequest request, CancellationToken cancellationToken);

    /// <summary>
    ///     Asynchronously decompresses the specified archive according
    ///     to the provided request parameters.
    /// </summary>
    /// <param name="request">
    ///     An object that specifies the archive to decompress and the
    ///     options to use during decompression.
    /// </param>
    /// <param name="cancellationToken">
    ///     A token that can be used to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result
    ///     contains a <see cref="DecompressResponse"/> with details
    ///     about the decompression outcome.
    /// </returns>
    Task<DecompressResponse> DecompressAsync(DecompressRequest request, CancellationToken cancellationToken);
}