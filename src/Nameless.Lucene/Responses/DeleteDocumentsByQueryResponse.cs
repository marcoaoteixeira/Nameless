using Nameless.Lucene.Requests;
using Nameless.ObjectModel;
using Nameless.Results;

namespace Nameless.Lucene.Responses;

/// <summary>
///     Represents the <see cref="DeleteDocumentsByQueryResponse"/> metadata.
/// </summary>
/// <param name="Count">
///     The number of deleted documents.
/// </param>
public readonly record struct DeleteDocumentsByQueryMetadata(int Count);

/// <summary>
///     Represents the response for a <see cref="DeleteDocumentsRequest"/>.
/// </summary>
public class DeleteDocumentsByQueryResponse : Result<DeleteDocumentsByQueryMetadata> {
    private DeleteDocumentsByQueryResponse(DeleteDocumentsByQueryMetadata value, Error[] errors)
        : base(value, errors) { }

    /// <summary>
    ///     Converts a <see cref="DeleteDocumentsByQueryMetadata"/> into a
    ///     <see cref="DeleteDocumentsByQueryResponse"/>.
    /// </summary>
    /// <param name="value">
    ///     The <see cref="DeleteDocumentsByQueryMetadata"/> instance.
    /// </param>
    public static implicit operator DeleteDocumentsByQueryResponse(DeleteDocumentsByQueryMetadata value) {
        return new DeleteDocumentsByQueryResponse(value, errors: []);
    }

    /// <summary>
    ///     Converts an array of <see cref="Error"/> into a
    ///     <see cref="DeleteDocumentsByQueryResponse"/>.
    /// </summary>
    /// <param name="errors">
    ///     The array of <see cref="Error"/> instance.
    /// </param>
    public static implicit operator DeleteDocumentsByQueryResponse(Error[] errors) {
        return new DeleteDocumentsByQueryResponse(value: default, errors);
    }

    /// <summary>
    ///     Converts a <see cref="Error"/> into a
    ///     <see cref="DeleteDocumentsByQueryResponse"/>.
    /// </summary>
    /// <param name="error">
    ///     The <see cref="Error"/> instance.
    /// </param>
    public static implicit operator DeleteDocumentsByQueryResponse(Error error) {
        return new DeleteDocumentsByQueryResponse(value: default, errors: [error]);
    }

    internal static Task<DeleteDocumentsByQueryResponse> From(int count) {
        return Task.FromResult<DeleteDocumentsByQueryResponse>(
            new DeleteDocumentsByQueryMetadata(count)
        );
    }

    internal static Task<DeleteDocumentsByQueryResponse> From(params Error[] errors) {
        return Task.FromResult<DeleteDocumentsByQueryResponse>(errors);
    }
}