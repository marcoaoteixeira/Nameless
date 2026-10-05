namespace Nameless.Compression;

/// <summary>
///     Outcome of a compression.
/// </summary>
/// <param name="FilePath">
///     Full path of the created archive.
/// </param>
/// <param name="Checksum">
///     Lowercase hex digest of the archive file, computed with
///     <paramref name="Algorithm"/>. Pass it to <c>DecompressAsync</c> to
///     detect tampering or corruption.
/// </param>
/// <param name="Algorithm">
///     <see cref="ChecksumAlgorithm.Sha256"/> proves integrity only if
///     the checksum reaches the consumer through a channel the archive's
///     sender cannot alter. <see cref="ChecksumAlgorithm.HmacSha256"/>
///     (compressor created with a key) also proves the archive was produced
///     by a holder of that key.
/// </param>
public sealed record CompressionResult(string FilePath, string Checksum, ChecksumAlgorithm Algorithm);