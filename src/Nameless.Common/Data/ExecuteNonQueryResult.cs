using Nameless.ObjectModel;

namespace Nameless.Data;

/// <summary>
///     Represents an execute non-query response.
/// </summary>
public sealed class ExecuteNonQueryResult : Result<int> {
    private ExecuteNonQueryResult(int? value, Error[] errors)
        : base(value, errors) { }

    /// <summary>
    ///     Converts a <see cref="int"/> value into a
    ///     <see cref="ExecuteNonQueryResult"/> instance.
    /// </summary>
    /// <param name="value">
    ///     The value.
    /// </param>
    public static implicit operator ExecuteNonQueryResult(int value) {
        return new ExecuteNonQueryResult(value, errors: []);
    }

    /// <summary>
    ///     Converts a <see cref="Error"/> value into a
    ///     <see cref="ExecuteNonQueryResult"/> instance.
    /// </summary>
    /// <param name="error">
    ///     The error.
    /// </param>
    public static implicit operator ExecuteNonQueryResult(Error error) {
        return new ExecuteNonQueryResult(value: null, errors: [error]);
    }
}