using Nameless.ObjectModel;
using Nameless.Results;

namespace Nameless.Data.Responses;

/// <summary>
///     Represents an execute reader response.
/// </summary>
public sealed class ExecuteReaderResponse<TResult> : Result<TResult[]> {
    private ExecuteReaderResponse(TResult[] value, Error[] errors)
        : base(value, errors) { }

    /// <summary>
    ///     Converts an array of <typeparamref name="TResult"/> into a
    ///     <see cref="ExecuteReaderResponse{TResult}"/> instance.
    /// </summary>
    /// <param name="value">
    ///     The value.
    /// </param>
    public static implicit operator ExecuteReaderResponse<TResult>(TResult[] value) {
        return new ExecuteReaderResponse<TResult>(value, errors: []);
    }

    /// <summary>
    ///     Converts a <see cref="Error"/> value into a
    ///     <see cref="ExecuteReaderResponse{TResult}"/> instance.
    /// </summary>
    /// <param name="error">
    ///     The error.
    /// </param>
    public static implicit operator ExecuteReaderResponse<TResult>(Error error) {
        return new ExecuteReaderResponse<TResult>(value: [], errors: [error]);
    }
}