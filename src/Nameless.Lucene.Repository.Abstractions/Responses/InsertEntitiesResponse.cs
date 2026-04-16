using Nameless.Lucene.Repository.Requests;
using Nameless.ObjectModel;
using Nameless.Results;

namespace Nameless.Lucene.Repository.Responses;

/// <summary>
///     Represents the <see cref="InsertEntitiesResponse"/> metadata.
/// </summary>
/// <param name="Count">
///     The number of inserted documents.
/// </param>
public readonly record struct InsertEntitiesMetadata(int Count);

/// <summary>
///     Represents the response for a <see cref="InsertEntitiesRequest{TEntity}"/>.
/// </summary>
public class InsertEntitiesResponse : Result<InsertEntitiesMetadata> {
    private InsertEntitiesResponse(InsertEntitiesMetadata value, Error[] errors)
        : base(value, errors) { }

    /// <summary>
    ///     Converts a <see cref="InsertEntitiesMetadata"/> into a
    ///     <see cref="InsertEntitiesResponse"/>.
    /// </summary>
    /// <param name="value">
    ///     The <see cref="InsertEntitiesMetadata"/> instance.
    /// </param>
    public static implicit operator InsertEntitiesResponse(InsertEntitiesMetadata value) {
        return new InsertEntitiesResponse(value, errors: []);
    }

    /// <summary>
    ///     Converts an array of <see cref="Error"/> into a
    ///     <see cref="InsertEntitiesResponse"/>.
    /// </summary>
    /// <param name="errors">
    ///     The array of <see cref="Error"/> instance.
    /// </param>
    public static implicit operator InsertEntitiesResponse(Error[] errors) {
        return new InsertEntitiesResponse(value: default, errors);
    }

    /// <summary>
    ///     Converts a <see cref="Error"/> into a
    ///     <see cref="InsertEntitiesResponse"/>.
    /// </summary>
    /// <param name="error">
    ///     The <see cref="Error"/> instance.
    /// </param>
    public static implicit operator InsertEntitiesResponse(Error error) {
        return new InsertEntitiesResponse(value: default, errors: [error]);
    }

    public static Task<InsertEntitiesResponse> From(int count) {
        return Task.FromResult<InsertEntitiesResponse>(
            new InsertEntitiesMetadata(count)
        );
    }

    public static Task<InsertEntitiesResponse> From(params Error[] errors) {
        return Task.FromResult<InsertEntitiesResponse>(errors);
    }
}