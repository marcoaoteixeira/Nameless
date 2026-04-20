using Nameless.ObjectModel;
using Nameless.Results;

namespace Nameless.Data.Responses;

/// <summary>
///     Represents an execute scalar response.
/// </summary>
public sealed class ExecuteScalarResponse<TResult> : Result<TResult?> {
    private ExecuteScalarResponse(TResult? value, Error[] errors)
        : base(value, errors) { }

    /// <summary>
    ///     Converts a <typeparamref name="TResult"/> value into a
    ///     <see cref="ExecuteScalarResponse{TResult}"/> instance.
    /// </summary>
    /// <param name="value">
    ///     The value.
    /// </param>
    public static implicit operator ExecuteScalarResponse<TResult>(TResult? value) {
        return new ExecuteScalarResponse<TResult>(value, errors: []);
    }

    /// <summary>
    ///     Converts a <see cref="Error"/> value into a
    ///     <see cref="ExecuteScalarResponse{TResult}"/> instance.
    /// </summary>
    /// <param name="error">
    ///     The error.
    /// </param>
    public static implicit operator ExecuteScalarResponse<TResult>(Error error) {
        return new ExecuteScalarResponse<TResult>(value: default, errors: [error]);
    }
}