using Nameless.ObjectModel;
using Nameless.Results;

namespace Nameless.Data.Responses;

public sealed class ExecuteNonQueryResponse : Result<int> {
    private ExecuteNonQueryResponse(int value, Error[] errors)
        : base(value, errors) { }

    public static implicit operator ExecuteNonQueryResponse(int value) {
        return new ExecuteNonQueryResponse(value, errors: []);
    }

    public static implicit operator ExecuteNonQueryResponse(Error error) {
        return new ExecuteNonQueryResponse(value: 0, errors: [error]);
    }
}