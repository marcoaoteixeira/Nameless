using Nameless.Lucene.Repository.Requests;
using Nameless.ObjectModel;
using Nameless.Results;

namespace Nameless.Lucene.Repository.Responses;

/// <summary>
///     Represents the <see cref="DeleteEntitiesResponse"/> metadata.
/// </summary>
/// <param name="Count">
///     The number of deleted documents.
/// </param>
public readonly record struct DeleteDocumentsMetadata(int Count);

/// <summary>
///     Represents the response for a <see cref="DeleteEntitiesRequest{TEntity}"/>.
/// </summary>
public class DeleteEntitiesResponse : Result<DeleteDocumentsMetadata> {
    private DeleteEntitiesResponse(DeleteDocumentsMetadata value, Error[] errors)
        : base(value, errors) { }

    /// <summary>
    ///     Converts a <see cref="DeleteDocumentsMetadata"/> into a
    ///     <see cref="DeleteEntitiesResponse"/>.
    /// </summary>
    /// <param name="value">
    ///     The <see cref="DeleteDocumentsMetadata"/> instance.
    /// </param>
    public static implicit operator DeleteEntitiesResponse(DeleteDocumentsMetadata value) {
        return new DeleteEntitiesResponse(value, errors: []);
    }

    /// <summary>
    ///     Converts an array of <see cref="Error"/> into a
    ///     <see cref="DeleteEntitiesResponse"/>.
    /// </summary>
    /// <param name="errors">
    ///     The array of <see cref="Error"/> instance.
    /// </param>
    public static implicit operator DeleteEntitiesResponse(Error[] errors) {
        return new DeleteEntitiesResponse(value: default, errors);
    }

    /// <summary>
    ///     Converts a <see cref="Error"/> into a
    ///     <see cref="DeleteEntitiesResponse"/>.
    /// </summary>
    /// <param name="error">
    ///     The <see cref="Error"/> instance.
    /// </param>
    public static implicit operator DeleteEntitiesResponse(Error error) {
        return new DeleteEntitiesResponse(value: default, errors: [error]);
    }

    /// <summary>
    ///     Creates a <see cref="Task{TResult}"/> where the result is an
    ///     instance of <see cref="DeleteEntitiesResponse"/>.
    /// </summary>
    /// <param name="count">
    ///     The number of deleted entities.
    /// </param>
    /// <returns>
    ///     A <see cref="Task{TResult}"/> where the result is an instance
    ///     of <see cref="DeleteEntitiesResponse"/>.
    /// </returns>
    public static Task<DeleteEntitiesResponse> From(int count) {
        return Task.FromResult<DeleteEntitiesResponse>(
            new DeleteDocumentsMetadata(count)
        );
    }

    /// <summary>
    ///     Creates a <see cref="Task{TResult}"/> where the result is an
    ///     instance of <see cref="DeleteEntitiesResponse"/>.
    /// </summary>
    /// <param name="errors">
    ///     The errors.
    /// </param>
    /// <returns>
    ///     A <see cref="Task{TResult}"/> where the result is an instance
    ///     of <see cref="DeleteEntitiesResponse"/>.
    /// </returns>
    public static Task<DeleteEntitiesResponse> From(params Error[] errors) {
        return Task.FromResult<DeleteEntitiesResponse>(errors);
    }
}