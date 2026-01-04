using Nameless.ObjectModel;
using Nameless.Results;

namespace Nameless.Lucene.Responses;

public readonly record struct DeleteDocumentsByQueryMetadata(int TotalDocumentsDeleted);

public sealed class DeleteDocumentsByQueryResponse : Result<DeleteDocumentsByQueryMetadata> {
    private DeleteDocumentsByQueryResponse(DeleteDocumentsByQueryMetadata value, Error[] errors)
        : base(value, errors) { }

    public static implicit operator DeleteDocumentsByQueryResponse(DeleteDocumentsByQueryMetadata value) {
        return new DeleteDocumentsByQueryResponse(value, errors: []);
    }

    public static implicit operator DeleteDocumentsByQueryResponse(Error error) {
        return new DeleteDocumentsByQueryResponse(value: default, errors: [error]);
    }

    public static Task<DeleteDocumentsByQueryResponse> From(int totalDocumentsDeleted) {
        return Task.FromResult<DeleteDocumentsByQueryResponse>(
            new DeleteDocumentsByQueryMetadata(totalDocumentsDeleted)
        );
    }

    public static Task<DeleteDocumentsByQueryResponse> From(Error error) {
        return Task.FromResult<DeleteDocumentsByQueryResponse>(error);
    }
}