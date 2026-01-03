using Nameless.ObjectModel;
using Nameless.Results;

namespace Nameless.Lucene.Responses;

public sealed class DeleteDocumentsByQueryResponse : Result<int> {
    private DeleteDocumentsByQueryResponse(int value, Error[] errors)
        : base(value, errors) { }

    public static implicit operator DeleteDocumentsByQueryResponse(int value) {
        return new DeleteDocumentsByQueryResponse(value, errors: []);
    }

    public static implicit operator DeleteDocumentsByQueryResponse(Error error) {
        return new DeleteDocumentsByQueryResponse(value: 0, errors: [error]);
    }
}