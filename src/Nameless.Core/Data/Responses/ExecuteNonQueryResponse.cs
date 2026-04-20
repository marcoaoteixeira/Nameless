using Nameless.ObjectModel;
using Nameless.Results;

namespace Nameless.Data.Responses;

/// <summary>
///     Represents an execute non-query response.
/// </summary>
public sealed class ExecuteNonQueryResponse : Result<int> {
    private ExecuteNonQueryResponse(int value, Error[] errors)
        : base(value, errors) { }

    /// <summary>
    ///     Converts a <see cref="int"/> value into a
    ///     <see cref="ExecuteNonQueryResponse"/> instance.
    /// </summary>
    /// <param name="value">
    ///     The value.
    /// </param>
    public static implicit operator ExecuteNonQueryResponse(int value) {
        return new ExecuteNonQueryResponse(value, errors: []);
    }

    /// <summary>
    ///     Converts a <see cref="Error"/> value into a
    ///     <see cref="ExecuteNonQueryResponse"/> instance.
    /// </summary>
    /// <param name="error">
    ///     The error.
    /// </param>
    public static implicit operator ExecuteNonQueryResponse(Error error) {
        return new ExecuteNonQueryResponse(value: 0, errors: [error]);
    }
}