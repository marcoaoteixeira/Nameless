namespace Nameless.Compression.Internals;

/// <summary>
///     CRC-32 (ISO-HDLC, polynomial 0xEDB88320) as used by ZIP. The BCL
///     keeps its implementation internal, and <c>System.IO.Hashing</c> is
///     a separate package, so this small table-driven version is used
///     instead.
/// </summary>
/// <example>
///     <code>
///         var crc = Crc32.Initial;
///         crc = Crc32.Update(crc, chunk1);
///         crc = Crc32.Update(crc, chunk2);
///         uint value = Crc32.Finish(crc);
///     </code>
/// </example>
internal static class Crc32 {
    private const uint POLYNOMIAL = 0xEDB88320;

    /// <summary>
    ///     Running value to start an incremental computation with.
    /// </summary>
    public const uint Initial = 0xFFFFFFFF;

    private static readonly uint[] Table = CreateTable();

    public static uint Update(uint crc, ReadOnlySpan<byte> data) {
        foreach (var value in data) {
            crc = Table[(crc ^ value) & 0xFF] ^ (crc >> 8);
        }

        return crc;
    }

    public static uint Finish(uint crc) {
        return ~crc;
    }

    public static uint Compute(ReadOnlySpan<byte> data) {
        return Finish(
            Update(Initial, data)
        );
    }

    private static uint[] CreateTable() {
        var table = new uint[256];

        for (uint index = 0; index < table.Length; index++) {
            var value = index;

            for (var bit = 0; bit < 8; bit++) {
                value = (value & 1) != 0
                    ? POLYNOMIAL ^ (value >> 1)
                    : value >> 1;
            }

            table[index] = value;
        }

        return table;
    }
}
