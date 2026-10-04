using Nameless.ObjectModel;

namespace Nameless.Data;

/// <summary>
///     Represents an execute reader response.
/// </summary>
public sealed class ExecuteReaderResult<T> : Result<T[]> {
    private ExecuteReaderResult(T[] value, Error[] errors)
        : base(value, errors) { }

    /// <summary>
    ///     Converts an array of <typeparamref name="T"/> into a
    ///     <see cref="ExecuteReaderResult{TResult}"/> instance.
    /// </summary>
    /// <param name="value">
    ///     The value.
    /// </param>
    public static implicit operator ExecuteReaderResult<T>(T[] value) {
        return new ExecuteReaderResult<T>(value, errors: []);
    }

    /// <summary>
    ///     Converts a <see cref="Error"/> value into a
    ///     <see cref="ExecuteReaderResult{TResult}"/> instance.
    /// </summary>
    /// <param name="error">
    ///     The error.
    /// </param>
    public static implicit operator ExecuteReaderResult<T>(Error error) {
        return new ExecuteReaderResult<T>(value: [], errors: [error]);
    }
}