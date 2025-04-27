namespace Nameless.Results;

public sealed class Result<TArg0, TArg1, TArg2> : ResultBase<TArg0, TArg1, TArg2> {
    private Result(int index, TArg0? arg0 = default, TArg1? arg1 = default, TArg2? arg2 = default, Error[]? errors = null)
        : base(index, arg0, arg1, arg2, errors) { }

    public static implicit operator Result<TArg0, TArg1, TArg2>(TArg0 arg0)
        => new(0, arg0: arg0);

    public static implicit operator Result<TArg0, TArg1, TArg2>(TArg1 arg1)
        => new(1, arg1: arg1);

    public static implicit operator Result<TArg0, TArg1, TArg2>(TArg2 arg2)
        => new(2, arg2: arg2);

    public static implicit operator Result<TArg0, TArg1, TArg2>(Error error)
        => new(3, errors: [error]);

    public static implicit operator Result<TArg0, TArg1, TArg2>(Error[] errors)
        => new(3, errors: errors);
}