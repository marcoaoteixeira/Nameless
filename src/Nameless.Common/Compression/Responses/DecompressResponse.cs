using Nameless.ObjectModel;
using Nameless.Results;

namespace Nameless.Compression.Responses;

/// <summary>
///     Represents a decompression response.
/// </summary>
public class DecompressResponse : Result<DecompressMetadata> {
    private DecompressResponse(DecompressMetadata value, Error[] errors)
        : base(value, errors) { }

    /// <summary>
    ///     Converts the <see cref="DecompressMetadata"/> into
    ///     a <see cref="DecompressResponse"/>.
    /// </summary>
    /// <param name="value">
    ///     The <see cref="DecompressMetadata"/> instance.
    /// </param>
    public static implicit operator DecompressResponse(DecompressMetadata value) {
        return new DecompressResponse(value, errors: []);
    }

    /// <summary>
    ///     Converts the <see cref="Error"/> array into
    ///     a <see cref="DecompressResponse"/>.
    /// </summary>
    /// <param name="errors">
    ///     The <see cref="Error"/> array instance.
    /// </param>
    public static implicit operator DecompressResponse(Error[] errors) {
        return new DecompressResponse(value: default, errors);
    }

    /// <summary>
    ///     Converts the <see cref="DecompressMetadata"/> into
    ///     a <see cref="DecompressResponse"/>.
    /// </summary>
    /// <param name="error">
    ///     The <see cref="Error"/> instance.
    /// </param>
    public static implicit operator DecompressResponse(Error error) {
        return new DecompressResponse(value: default, errors: [error]);
    }
}