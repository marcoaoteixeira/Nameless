using Nameless.Lucene.Repository.Requests;
using Nameless.ObjectModel;
using Nameless.Results;

namespace Nameless.Lucene.Repository.Responses;

/// <summary>
///     Represents the <see cref="DeleteEntitiesByQueryResponse"/> metadata.
/// </summary>
/// <param name="Count">
///     The number of deleted documents.
/// </param>
public readonly record struct DeleteDocumentsByQueryMetadata(int Count);

/// <summary>
///     Represents the response for a <see cref="DeleteEntitiesRequest{TEntity}"/>.
/// </summary>
public class DeleteEntitiesByQueryResponse : Result<DeleteDocumentsByQueryMetadata> {
    private DeleteEntitiesByQueryResponse(DeleteDocumentsByQueryMetadata value, Error[] errors)
        : base(value, errors) { }

    /// <summary>
    ///     Converts a <see cref="DeleteDocumentsByQueryMetadata"/> into a
    ///     <see cref="DeleteEntitiesByQueryResponse"/>.
    /// </summary>
    /// <param name="value">
    ///     The <see cref="DeleteDocumentsByQueryMetadata"/> instance.
    /// </param>
    public static implicit operator DeleteEntitiesByQueryResponse(DeleteDocumentsByQueryMetadata value) {
        return new DeleteEntitiesByQueryResponse(value, errors: []);
    }

    /// <summary>
    ///     Converts an array of <see cref="Error"/> into a
    ///     <see cref="DeleteEntitiesByQueryResponse"/>.
    /// </summary>
    /// <param name="errors">
    ///     The array of <see cref="Error"/> instance.
    /// </param>
    public static implicit operator DeleteEntitiesByQueryResponse(Error[] errors) {
        return new DeleteEntitiesByQueryResponse(value: default, errors);
    }

    /// <summary>
    ///     Converts a <see cref="Error"/> into a
    ///     <see cref="DeleteEntitiesByQueryResponse"/>.
    /// </summary>
    /// <param name="error">
    ///     The <see cref="Error"/> instance.
    /// </param>
    public static implicit operator DeleteEntitiesByQueryResponse(Error error) {
        return new DeleteEntitiesByQueryResponse(value: default, errors: [error]);
    }

    /// <summary>
    ///     Creates a successful response with the number of deleted documents.
    /// </summary>
    /// <param name="count">The number of documents deleted.</param>
    /// <returns>A <see cref="Task{TResult}"/> containing the successful response.</returns>
    public static Task<DeleteEntitiesByQueryResponse> From(int count) {
        return Task.FromResult<DeleteEntitiesByQueryResponse>(
            new DeleteDocumentsByQueryMetadata(count)
        );
    }

    /// <summary>
    ///     Creates a failure response from the given errors.
    /// </summary>
    /// <param name="errors">One or more <see cref="Error"/> instances describing the failures.</param>
    /// <returns>A <see cref="Task{TResult}"/> containing the failure response.</returns>
    public static Task<DeleteEntitiesByQueryResponse> From(params Error[] errors) {
        return Task.FromResult<DeleteEntitiesByQueryResponse>(errors);
    }
}