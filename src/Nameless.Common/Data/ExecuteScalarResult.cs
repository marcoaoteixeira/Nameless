using Nameless.ObjectModel;

namespace Nameless.Data;

/// <summary>
///     Represents an execute scalar response.
/// </summary>
public sealed class ExecuteScalarResult<T> : Result<T?> {
    private ExecuteScalarResult(T? value, Error[] errors)
        : base(value, errors) { }

    /// <summary>
    ///     Converts a <typeparamref name="T"/> value into a
    ///     <see cref="ExecuteScalarResult{TResult}"/> instance.
    /// </summary>
    /// <param name="value">
    ///     The value.
    /// </param>
    public static implicit operator ExecuteScalarResult<T>(T? value) {
        return new ExecuteScalarResult<T>(value, errors: []);
    }

    /// <summary>
    ///     Converts a <see cref="Error"/> value into a
    ///     <see cref="ExecuteScalarResult{TResult}"/> instance.
    /// </summary>
    /// <param name="error">
    ///     The error.
    /// </param>
    public static implicit operator ExecuteScalarResult<T>(Error error) {
        return new ExecuteScalarResult<T>(value: default, errors: [error]);
    }
}