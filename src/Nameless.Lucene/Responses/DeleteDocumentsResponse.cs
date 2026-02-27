using Nameless.Lucene.Requests;
using Nameless.ObjectModel;
using Nameless.Results;

namespace Nameless.Lucene.Responses;

/// <summary>
///     Represents the <see cref="DeleteDocumentsResponse"/> metadata.
/// </summary>
/// <param name="Count">
///     The number of deleted documents.
/// </param>
public readonly record struct DeleteDocumentsMetadata(int Count);

/// <summary>
///     Represents the response for a <see cref="DeleteDocumentsRequest"/>.
/// </summary>
public class DeleteDocumentsResponse : Result<DeleteDocumentsMetadata> {
    private DeleteDocumentsResponse(DeleteDocumentsMetadata value, Error[] errors)
        : base(value, errors) { }

    /// <summary>
    ///     Converts a <see cref="DeleteDocumentsMetadata"/> into a
    ///     <see cref="DeleteDocumentsResponse"/>.
    /// </summary>
    /// <param name="value">
    ///     The <see cref="DeleteDocumentsMetadata"/> instance.
    /// </param>
    public static implicit operator DeleteDocumentsResponse(DeleteDocumentsMetadata value) {
        return new DeleteDocumentsResponse(value, errors: []);
    }

    /// <summary>
    ///     Converts an array of <see cref="Error"/> into a
    ///     <see cref="DeleteDocumentsResponse"/>.
    /// </summary>
    /// <param name="errors">
    ///     The array of <see cref="Error"/> instance.
    /// </param>
    public static implicit operator DeleteDocumentsResponse(Error[] errors) {
        return new DeleteDocumentsResponse(value: default, errors);
    }

    /// <summary>
    ///     Converts a <see cref="Error"/> into a
    ///     <see cref="DeleteDocumentsResponse"/>.
    /// </summary>
    /// <param name="error">
    ///     The <see cref="Error"/> instance.
    /// </param>
    public static implicit operator DeleteDocumentsResponse(Error error) {
        return new DeleteDocumentsResponse(value: default, errors: [error]);
    }

    internal static Task<DeleteDocumentsResponse> From(int count) {
        return Task.FromResult<DeleteDocumentsResponse>(
            new DeleteDocumentsMetadata(count)
        );
    }
    
    internal static Task<DeleteDocumentsResponse> From(params Error[] errors) {
        return Task.FromResult<DeleteDocumentsResponse>(errors);
    }
}