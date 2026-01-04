using Nameless.ObjectModel;
using Nameless.Results;

namespace Nameless.Lucene.Responses;

public readonly record struct InsertDocumentsMetadata(int TotalDocumentsInserted);

public sealed class InsertDocumentsResponse : Result<InsertDocumentsMetadata> {
    private InsertDocumentsResponse(InsertDocumentsMetadata value, Error[] errors)
        : base(value, errors) { }

    public static implicit operator InsertDocumentsResponse(InsertDocumentsMetadata value) {
        return new InsertDocumentsResponse(value, errors: []);
    }

    public static implicit operator InsertDocumentsResponse(Error error) {
        return new InsertDocumentsResponse(value: default, errors: [error]);
    }

    public static Task<InsertDocumentsResponse> From(int totalDocumentsInserted) {
        return Task.FromResult<InsertDocumentsResponse>(
            new InsertDocumentsMetadata(totalDocumentsInserted)
        );
    }

    public static Task<InsertDocumentsResponse> From(Error error) {
        return Task.FromResult<InsertDocumentsResponse>(error);
    }
}