namespace Nameless.Results;

/// <summary>
///     Default result type with one argument of type
///     <typeparamref name="TResult"/>.
/// </summary>
/// <typeparam name="TResult">
///     Type of the result.
/// </typeparam>
public sealed class Result<TResult> : ResultBase<TResult> {
    private Result(int index, TResult? result = default, Error[]? errors = null)
        : base(index, result, errors) {
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
        return new Result<TResult>(index: 1, errors: [error]);
    }

    /// <summary>
    ///     Creates a failure result with the specified errors.
    /// </summary>
    /// <param name="errors">
    ///     The error collection.
    /// </param>
    public static implicit operator Result<TResult>(Error[] errors) {
        return new Result<TResult>(index: 1, errors: errors);
    }
}