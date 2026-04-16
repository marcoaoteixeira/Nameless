using Nameless.ObjectModel;
using Nameless.Results;

namespace Nameless.Compression.Responses;

/// <summary>
///     Represents a compress response.
/// </summary>
public class CompressResponse : Result<CompressMetadata> {
    private CompressResponse(CompressMetadata value, Error[] errors)
        : base(value, errors) {
    }

    /// <summary>
    ///     Converts the <see cref="CompressMetadata"/> into
    ///     a <see cref="CompressResponse"/>.
    /// </summary>
    /// <param name="value">
    ///     The <see cref="CompressMetadata"/> instance.
    /// </param>
    public static implicit operator CompressResponse(CompressMetadata value) {
        return new CompressResponse(value, errors: []);
    }

    /// <summary>
    ///     Converts the <see cref="Error"/> array into
    ///     a <see cref="CompressResponse"/>.
    /// </summary>
    /// <param name="errors">
    ///     The <see cref="Error"/> array instance.
    /// </param>
    public static implicit operator CompressResponse(Error[] errors) {
        return new CompressResponse(value: default, errors);
    }

    /// <summary>
    ///     Converts the <see cref="Error"/> into
    ///     a <see cref="CompressResponse"/>.
    /// </summary>
    /// <param name="error">
    ///     The <see cref="Error"/> instance.
    /// </param>
    public static implicit operator CompressResponse(Error error) {
        return new CompressResponse(value: default, errors: [error]);
    }
}