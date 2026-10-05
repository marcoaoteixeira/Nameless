namespace Nameless.Compression;

/// <summary>
///     How a <see cref="CompressionResult.Checksum"/> was computed.
/// </summary>
public enum ChecksumAlgorithm {
    /// <summary>
    ///     Plain SHA-256: detects corruption and tampering, but anyone
    ///     can recompute it.
    /// </summary>
    Sha256,

    /// <summary>
    ///     HMAC-SHA256 with the compressor's secret key: only key's holders
    ///     can produce a valid value.
    /// </summary>
    HmacSha256,
}