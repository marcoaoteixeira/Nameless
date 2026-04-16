using Nameless.ObjectModel;
using Nameless.Results;

namespace Nameless.Data.Responses;

public sealed class ExecuteReaderResponse<TResult> : Result<TResult[]> {
    private ExecuteReaderResponse(TResult[] value, Error[] errors)
        : base(value, errors) { }

    public static implicit operator ExecuteReaderResponse<TResult>(TResult[] value) {
        return new ExecuteReaderResponse<TResult>(value, errors: []);
    }

    public static implicit operator ExecuteReaderResponse<TResult>(Error error) {
        return new ExecuteReaderResponse<TResult>(value: [], errors: [error]);
    }
}