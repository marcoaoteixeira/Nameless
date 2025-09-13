namespace Nameless.Results;

/// <summary>
///     Default result type with one argument of type
///     <typeparamref name="TResult"/>.
/// </summary>
/// <typeparam name="TResult">
///     Type of the result.
/// </typeparam>
public sealed class Result<TResult> : ResultBase<TResult> {
    private Result(int index, TResult? result = default, Error error = default)
        : base(index, result, error) {
    }

    /// <summary>
    ///     Creates a successful result with the specified result.
    /// </summary>
    /// <param name="result">
    ///     The result.
    /// </param>
    public static implicit operator Result<TResult>(TResult result) {
        return new Result<TResult>(index: 0, result);
    }

    /// <summary>
    ///     Creates a failure result with the specified error.
    /// </summary>
    /// <param name="error">
    ///     The error.
    /// </param>
    public static implicit operator Result<TResult>(Error error) {
        return new Result<TResult>(index: 1, error: error);
    }
}