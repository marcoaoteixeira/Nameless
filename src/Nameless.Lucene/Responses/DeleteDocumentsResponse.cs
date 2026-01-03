using Nameless.ObjectModel;
using Nameless.Results;

namespace Nameless.Lucene.Responses;

public sealed class DeleteDocumentsResponse : Result<int> {
    private DeleteDocumentsResponse(int value, Error[] errors)
        : base(value, errors) { }

    public static implicit operator DeleteDocumentsResponse(int value) {
        return new DeleteDocumentsResponse(value, errors: []);
    }

    public static implicit operator DeleteDocumentsResponse(Error error) {
        return new DeleteDocumentsResponse(value: 0, errors: [error]);
    }
}