namespace Nameless.Compression;

/// <summary>
///     <see cref="ZipCompressor"/> options
/// </summary>
/// <remarks>
///     A class rather than a record on purpose: a record's generated
///     <see cref="object.ToString"/> prints every property, which would leak
///     <see cref="HmacKey"/> into logs.
/// </remarks>
public sealed class ZipCompressorOptions {
    /// <summary>
    ///     Default limits: 10,000 entries, 1 GiB in total, 100:1 ratio for
    ///     entries above 1 MiB.
    /// </summary>
    public static ZipCompressorOptions Default { get; } = new();

    /// <summary>
    ///     Gets or sets the secret key, as standard Base64 of at least 32
    ///     random bytes — generate one with <c>openssl rand -base64 32</c>
    ///     or <c>Convert.ToBase64String(RandomNumberGenerator.GetBytes(32))</c>.
    ///     Never a passphrase: the text is decoded as Base64, not as UTF-8.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         When set, checksums are HMAC-SHA256 instead of SHA-256, so only
    ///         holders of the key can produce archives that pass verification.
    ///         Producers and verifiers must share the same key; rotating it
    ///         invalidates the checksums of archives made with the old one.
    ///     </para>
    ///     <para>
    ///         <see langword="null"/> disables HMAC. An empty or white space
    ///         value is rejected rather than treated as "no key", since it
    ///         usually means a secret failed to load. White space and line
    ///         breaks around a valid value are ignored.
    ///     </para>
    /// </remarks>
    public string? HmacKey { get; init; }

    /// <summary>
    ///     Gets or sets the extraction limits options.
    /// </summary>
    public ExtractionLimits Limits {
        get;
        init => field = Throws.When.Null(value, paramName: nameof(Limits));
    } = new();
}

/// <summary>
///     Zip-bomb protection applied while extracting. Limits are enforced on
///     the bytes actually written, not on the sizes an archive declares
///     about itself (headers can lie). Exceeding any limit throws
///     <see cref="InvalidDataException"/>.
/// </summary>
public sealed record ExtractionLimits {
    /// <summary>
    ///     Gets or sets the maximum number of entries (files and directories)
    ///     in an archive.
    /// </summary>
    public int MaxEntryCount {
        get;
        init => field = Throws.When.LowerOrEqual(
            value,
            compare: 0,
            paramName: nameof(MaxEntryCount)
        );
    } = 10_000;

    /// <summary>
    ///     Gets or sets the maximum number of bytes written across all
    ///     entries.
    /// </summary>
    public long MaxTotalUncompressedBytes {
        get;
        init => field = Throws.When.LowerOrEqual(
            value,
            compare: 0,
            paramName: nameof(MaxTotalUncompressedBytes)
        );
    } = 1L << 30;

    /// <summary>
    ///     Gets or sets the maximum uncompressed-to-compressed size ratio of
    ///     a single entry.
    /// </summary>
    public double MaxCompressionRatio {
        get;
        init => field = Throws.When.OutOfRange(
            value,
            minimumValue: 1,
            maximumValue: double.PositiveInfinity,
            excludeBounds: true,
            message: "The ratio must be a finite number of at least 1.",
            paramName: nameof(MaxCompressionRatio)
        );
    } = 100;

    /// <summary>
    ///     Gets or sets the compression ratio threshold.
    /// </summary>
    /// <remarks>
    ///     Entries are only subject to <see cref="MaxCompressionRatio"/> once
    ///     they exceed this many bytes, so small, highly repetitive files
    ///     (which legitimately compress far beyond 100:1) are not rejected.
    /// </remarks>
    public long CompressionRatioThresholdBytes {
        get;
        init => field = Throws.When.LowerThan(
            value,
            compare: 0,
            paramName: nameof(CompressionRatioThresholdBytes)
        );
    } = 1L << 20;
}