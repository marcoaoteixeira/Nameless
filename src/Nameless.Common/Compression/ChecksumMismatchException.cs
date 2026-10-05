namespace Nameless.Compression;

/// <summary>
///     Thrown when an archive's checksum (SHA-256 or HMAC-SHA256) does not
///     match the value the caller expected.
/// </summary>
/// <remarks>
///     Derives from <see cref="Exception"/> on purpose:
///     <see cref="InvalidDataException"/> is sealed, and deriving from
///     <see cref="IOException"/> would let handlers meant for file conflicts
///     silently swallow integrity failures.
/// </remarks>
public sealed class ChecksumMismatchException : Exception {
    /// <summary>
    ///     Gets the checksum supplied by the caller.
    /// </summary>
    public string Expected { get; }

    /// <summary>
    ///     Gets the checksum computed from the archive (lowercase hex),
    ///     or <see langword="null"/> for HMAC-SHA256: revealing the correct
    ///     HMAC of attacker-supplied data would let the attacker forge it.
    /// </summary>
    public string? Actual { get; }

    /// <summary>
    ///     Initializes a new instance of
    ///     <see cref="ChecksumMismatchException"/> class.
    /// </summary>
    /// <param name="expected">
    ///     The expected checksum.
    /// </param>
    /// <param name="actual">
    ///     The actual checksum.
    /// </param>
    public ChecksumMismatchException(string expected, string? actual) : base(GetMessage(expected, actual)) {
        Expected = expected;
        Actual = actual;
    }

    private static string GetMessage(string expected, string? actual) {
        return actual is null
            ? $"Archive checksum mismatch: expected '{expected}'. The archive may have been tampered with or corrupted."
            : $"Archive checksum mismatch: expected '{expected}', computed '{actual}'. The archive may have been tampered with or corrupted.";
    }
}
