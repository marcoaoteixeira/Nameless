using Nameless.Lucene.Requests;
using Nameless.ObjectModel;
using Nameless.Results;

namespace Nameless.Lucene.Responses;

/// <summary>
///     Represents the <see cref="InsertDocumentsResponse"/> metadata.
/// </summary>
/// <param name="Count">
///     The number of inserted documents.
/// </param>
public readonly record struct InsertDocumentsMetadata(int Count);

/// <summary>
///     Represents the response for a <see cref="InsertDocumentsRequest{TDocument}"/>.
/// </summary>
public class InsertDocumentsResponse : Result<InsertDocumentsMetadata> {
    private InsertDocumentsResponse(InsertDocumentsMetadata value, Error[] errors)
        : base(value, errors) { }

    /// <summary>
    ///     Converts a <see cref="InsertDocumentsMetadata"/> into a
    ///     <see cref="InsertDocumentsResponse"/>.
    /// </summary>
    /// <param name="value">
    ///     The <see cref="InsertDocumentsMetadata"/> instance.
    /// </param>
    public static implicit operator InsertDocumentsResponse(InsertDocumentsMetadata value) {
        return new InsertDocumentsResponse(value, errors: []);
    }

    /// <summary>
    ///     Converts an array of <see cref="Error"/> into a
    ///     <see cref="InsertDocumentsResponse"/>.
    /// </summary>
    /// <param name="errors">
    ///     The array of <see cref="Error"/> instance.
    /// </param>
    public static implicit operator InsertDocumentsResponse(Error[] errors) {
        return new InsertDocumentsResponse(value: default, errors);
    }

    /// <summary>
    ///     Converts a <see cref="Error"/> into a
    ///     <see cref="InsertDocumentsResponse"/>.
    /// </summary>
    /// <param name="error">
    ///     The <see cref="Error"/> instance.
    /// </param>
    public static implicit operator InsertDocumentsResponse(Error error) {
        return new InsertDocumentsResponse(value: default, errors: [error]);
    }

    internal static Task<InsertDocumentsResponse> From(int count) {
        return Task.FromResult<InsertDocumentsResponse>(
            new InsertDocumentsMetadata(count)
        );
    }

    internal static Task<InsertDocumentsResponse> From(params Error[] errors) {
        return Task.FromResult<InsertDocumentsResponse>(errors);
    }
}