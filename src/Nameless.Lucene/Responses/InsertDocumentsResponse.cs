using Nameless.ObjectModel;
using Nameless.Results;

namespace Nameless.Lucene.Responses;

public sealed class InsertDocumentsResponse : Result<int> {
    private InsertDocumentsResponse(int value, Error[] errors)
        : base(value, errors) { }

    public static implicit operator InsertDocumentsResponse(int value) {
        return new InsertDocumentsResponse(value, errors: []);
    }

    public static implicit operator InsertDocumentsResponse(Error error) {
        return new InsertDocumentsResponse(value: 0, errors: [error]);
    }
}