using Nameless.ObjectModel;
using Nameless.Results;

namespace Nameless.Lucene.Responses;

public readonly record struct DeleteDocumentsMetadata(int TotalDocumentsDeleted);

public sealed class DeleteDocumentsResponse : Result<DeleteDocumentsMetadata> {
    private DeleteDocumentsResponse(DeleteDocumentsMetadata value, Error[] errors)
        : base(value, errors) { }

    public static implicit operator DeleteDocumentsResponse(DeleteDocumentsMetadata value) {
        return new DeleteDocumentsResponse(value, errors: []);
    }

    public static implicit operator DeleteDocumentsResponse(Error error) {
        return new DeleteDocumentsResponse(value: default, errors: [error]);
    }

    public static Task<DeleteDocumentsResponse> From(int totalDocumentsDeleted) {
        return Task.FromResult<DeleteDocumentsResponse>(
            new DeleteDocumentsMetadata(totalDocumentsDeleted)
        );
    }

    public static Task<DeleteDocumentsResponse> From(Error error) {
        return Task.FromResult<DeleteDocumentsResponse>(error);
    }
}