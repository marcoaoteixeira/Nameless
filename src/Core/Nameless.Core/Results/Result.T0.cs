namespace Nameless.Results;

public sealed class Result<TArg0> : ResultBase<TArg0> {
    private Result(int index, TArg0? arg0 = default, Error[]? errors = null)
        : base(index, arg0, errors) { }

    public static implicit operator Result<TArg0>(TArg0 arg0)
        => new(0, arg0: arg0);

    public static implicit operator Result<TArg0>(Error error)
        => new(1, errors: [error]);

    public static implicit operator Result<TArg0>(Error[] errors)
        => new(1, errors: errors);
}