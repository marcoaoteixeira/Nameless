using System.Buffers;
using System.Buffers.Text;
using System.Runtime.InteropServices;

namespace Nameless.Helpers;

/// <summary>
///     Insanely fast encode/decode GUID helper.
/// </summary>
public static class GuidHelper {
    private const char EQUALS = '=';
    private const char HYPHEN = '-';
    private const char UNDERSCORE = '_';
    private const char FORWARD_SLASH = '/';
    private const char PLUS = '+';

    /// <summary>
    ///     Encodes the <see cref="Guid"/> value as string.
    /// </summary>
    /// <param name="value">
    ///     The <see cref="Guid"/>.
    /// </param>
    /// <returns>
    ///     A <see cref="string"/> representing the <see cref="Guid"/> value.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     If it was not possible to encode the value.
    /// </exception>
    public static string Encode(Guid value) {
        // Why 16 bytes? See (Binary wire format): https://en.wikipedia.org/wiki/Universally_unique_identifier

        // The value parameter representation as bytes
        Span<byte> valueBuffer = stackalloc byte[16];

        // The base64 representation as bytes
        Span<byte> base64Buffer = stackalloc byte[24];

        var writtenSuccessfully = MemoryMarshal.TryWrite(valueBuffer, in value);
        if (!writtenSuccessfully) {
            throw new InvalidOperationException($"Couldn't write to {nameof(valueBuffer)}.");
        }

        // The only place that we allocate memory
        var status = Base64.EncodeToUtf8(valueBuffer, base64Buffer, out _, out _);

        if (status != OperationStatus.Done) {
            throw new InvalidOperationException($"Couldn't encode {nameof(valueBuffer)}.");
        }

        Span<char> chars = stackalloc char[22];

        for (var idx = 0; idx < chars.Length; idx++) {
            chars[idx] = base64Buffer[idx] switch {
                (byte)FORWARD_SLASH => HYPHEN,
                (byte)PLUS => UNDERSCORE,
                _ => (char)base64Buffer[idx]
            };
        }

        return new string(chars);
    }

    /// <summary>
    ///     Decodes the span value into a <see cref="Guid"/> value.
    /// </summary>
    /// <param name="value">
    ///     The span.
    /// </param>
    /// <returns>
    ///     The decoded <see cref="Guid"/>.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     If it was not possible to decode the value.
    /// </exception>
    public static Guid Decode(ReadOnlySpan<char> value) {
        Span<char> base64Buffer = stackalloc char[24];

        // There should be 2 equals signs at the end of decoded value
        // will skip them here.
        for (var idx = 0; idx < base64Buffer.Length - 2; idx++) {
            base64Buffer[idx] = value[idx] switch {
                HYPHEN => FORWARD_SLASH,
                UNDERSCORE => PLUS,
                _ => value[idx]
            };
        }

        base64Buffer[index: 22] = EQUALS;
        base64Buffer[index: 23] = EQUALS;

        Span<byte> result = stackalloc byte[16];

        return Convert.TryFromBase64Chars(base64Buffer, result, out _)
            ? new Guid(result)
            : throw new InvalidOperationException(
                "Couldn't convert from Base64 chars."
            );
    }
}