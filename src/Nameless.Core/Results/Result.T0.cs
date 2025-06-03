namespace Nameless.Results;

/// <summary>
/// Default result type with one argument of type <typeparamref name="TArg0"/>.
/// </summary>
/// <typeparam name="TArg0">Type of the argument at position 0.</typeparam>
public sealed class Result<TArg0> : ResultBase<TArg0> {
    private Result(int index, TArg0? arg0 = default, Error[]? errors = null)
        : base(index, arg0, errors) {
    }

    /// <summary>
    /// Creates a successful result with the specified argument at position 0.
    /// </summary>
    /// <param name="arg0">The argument at position 0.</param>
    public static implicit operator Result<TArg0>(TArg0 arg0)
        => new(index: 0, arg0);

    /// <summary>
    /// Creates a result with an error at position 1.
    /// </summary>
    /// <param name="error"></param>
    public static implicit operator Result<TArg0>(Error error)
        => new(index: 1, errors: [error]);

    /// <summary>
    /// Creates a result with an array of errors at position 1.
    /// </summary>
    /// <param name="errors"></param>
    public static implicit operator Result<TArg0>(Error[] errors)
        => new(index: 1, errors: errors);
}