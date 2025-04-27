namespace Nameless.Results;

public sealed class Result<TArg0, TArg1> : ResultBase<TArg0, TArg1> {
    private Result(int index, TArg0? arg0 = default, TArg1? arg1 = default, Error[]? errors = null)
        : base(index, arg0, arg1, errors) { }

    public static implicit operator Result<TArg0, TArg1>(TArg0 arg0)
        => new(0, arg0: arg0);

    public static implicit operator Result<TArg0, TArg1>(TArg1 arg1)
        => new(1, arg1: arg1);

    public static implicit operator Result<TArg0, TArg1>(Error error)
        => new(2, errors: [error]);

    public static implicit operator Result<TArg0, TArg1>(Error[] errors)
        => new(2, errors: errors);
}