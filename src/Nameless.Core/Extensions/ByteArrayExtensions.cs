namespace Nameless;

/// <summary>
///     <see cref="byte" /> array extension methods.
/// </summary>
public static class ByteArrayExtensions {
    /// <summary>
    ///     Transforms an array of <see cref="byte" />s into a hexadecimal <see cref="string" /> representation.
    /// </summary>
    /// <param name="self">The source array of <see cref="byte" />.</param>
    /// <returns>A hexadecimal <see cref="string" /> representation of the <see cref="byte" /> array.</returns>
    public static string ToHexString(this byte[] self) {
        return BitConverter.ToString(self).Replace("-", string.Empty);
    }

    /// <summary>
    ///     <strong>(Syntax sugar)</strong> Convert a byte array to a Base64 string.
    /// </summary>
    /// <param name="self">The current byte array.</param>
    /// <param name="options">The convert options.</param>
    /// <returns>
    ///     A base64 string representation for the current array.
    /// </returns>
    public static string ToBase64String(this byte[] self, Base64FormattingOptions options = Base64FormattingOptions.None) {
        return Convert.ToBase64String(self, options);
    }
}