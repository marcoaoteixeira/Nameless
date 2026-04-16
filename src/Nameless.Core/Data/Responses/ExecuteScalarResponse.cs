using Nameless.ObjectModel;
using Nameless.Results;

namespace Nameless.Data.Responses;

public sealed class ExecuteScalarResponse<TResult> : Result<TResult?> {
    private ExecuteScalarResponse(TResult? value, Error[] errors)
        : base(value, errors) { }

    public static implicit operator ExecuteScalarResponse<TResult>(TResult? value) {
        return new ExecuteScalarResponse<TResult>(value, errors: []);
    }

    public static implicit operator ExecuteScalarResponse<TResult>(Error error) {
        return new ExecuteScalarResponse<TResult>(value: default, errors: [error]);
    }
}