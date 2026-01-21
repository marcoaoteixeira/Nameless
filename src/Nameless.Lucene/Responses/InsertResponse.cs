using Nameless.ObjectModel;
using Nameless.Results;

namespace Nameless.Lucene.Responses;

public readonly record struct InsertDocumentsMetadata(int Count);

public class InsertResponse : Result<InsertDocumentsMetadata> {
    private InsertResponse(InsertDocumentsMetadata value, Error[] errors)
        : base(value, errors) { }

    public static implicit operator InsertResponse(InsertDocumentsMetadata value) {
        return new InsertResponse(value, errors: []);
    }

    public static implicit operator InsertResponse(Error error) {
        return new InsertResponse(value: default, errors: [error]);
    }

    public static Task<InsertResponse> From(int count) {
        return Task.FromResult<InsertResponse>(
            new InsertDocumentsMetadata(count)
        );
    }

    public static Task<InsertResponse> From(Error error) {
        return Task.FromResult<InsertResponse>(error);
    }
}