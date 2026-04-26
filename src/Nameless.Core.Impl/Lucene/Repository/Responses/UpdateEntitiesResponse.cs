using Nameless.Lucene.Repository.Requests;
using Nameless.ObjectModel;
using Nameless.Results;

namespace Nameless.Lucene.Repository.Responses;

/// <summary>
///     Represents the <see cref="UpdateEntitiesResponse"/> metadata.
/// </summary>
/// <param name="Count">
///     The number of inserted documents.
/// </param>
public readonly record struct UpdateDocumentsMetadata(int Count);

/// <summary>
///     Represents the response for a <see cref="UpdateEntitiesRequest{TEntity}"/>.
/// </summary>
public class UpdateEntitiesResponse : Result<UpdateDocumentsMetadata> {
    private UpdateEntitiesResponse(UpdateDocumentsMetadata value, Error[] errors)
        : base(value, errors) { }

    /// <summary>
    ///     Converts a <see cref="UpdateDocumentsMetadata"/> into a
    ///     <see cref="UpdateEntitiesResponse"/>.
    /// </summary>
    /// <param name="value">
    ///     The <see cref="UpdateDocumentsMetadata"/> instance.
    /// </param>
    public static implicit operator UpdateEntitiesResponse(UpdateDocumentsMetadata value) {
        return new UpdateEntitiesResponse(value, errors: []);
    }

    /// <summary>
    ///     Converts an array of <see cref="Error"/> into a
    ///     <see cref="UpdateEntitiesResponse"/>.
    /// </summary>
    /// <param name="errors">
    ///     The array of <see cref="Error"/> instance.
    /// </param>
    public static implicit operator UpdateEntitiesResponse(Error[] errors) {
        return new UpdateEntitiesResponse(value: default, errors);
    }

    /// <summary>
    ///     Converts a <see cref="Error"/> into a
    ///     <see cref="UpdateEntitiesResponse"/>.
    /// </summary>
    /// <param name="error">
    ///     The <see cref="Error"/> instance.
    /// </param>
    public static implicit operator UpdateEntitiesResponse(Error error) {
        return new UpdateEntitiesResponse(value: default, errors: [error]);
    }

    public static Task<UpdateEntitiesResponse> From(int count) {
        return Task.FromResult<UpdateEntitiesResponse>(
            new UpdateDocumentsMetadata(count)
        );
    }

    public static Task<UpdateEntitiesResponse> From(params Error[] errors) {
        return Task.FromResult<UpdateEntitiesResponse>(errors);
    }
}