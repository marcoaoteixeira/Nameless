using Nameless.ObjectModel;
using Nameless.Results;

namespace Nameless.Lucene.Responses;

public readonly record struct DeleteDocumentsMetadata(int Count);

public class DeleteDocumentsResponse : Result<DeleteDocumentsMetadata> {
    private DeleteDocumentsResponse(DeleteDocumentsMetadata value, Error[] errors)
        : base(value, errors) { }

    public static implicit operator DeleteDocumentsResponse(DeleteDocumentsMetadata value) {
        return new DeleteDocumentsResponse(value, errors: []);
    }

    public static implicit operator DeleteDocumentsResponse(Error error) {
        return new DeleteDocumentsResponse(value: default, errors: [error]);
    }

    public static Task<DeleteDocumentsResponse> From(int count) {
        return Task.FromResult<DeleteDocumentsResponse>(
            new DeleteDocumentsMetadata(count)
        );
    }

    public static Task<DeleteDocumentsResponse> From(Error error) {
        return Task.FromResult<DeleteDocumentsResponse>(error);
    }
}