namespace Nameless.Results;

public readonly struct Result<TArg0> : IResult {
    private readonly TArg0? _arg0;
    private readonly Error[]? _errors;

    public object? Value
        => Index switch {
            0 => _arg0,
            1 => _errors,
            _ => throw new InvalidOperationException("Invalid index")
        };
    public int Index { get; }
    public bool IsArg0 => Index == 0;
    public bool HasErrors => Index == 1;
    public TArg0? AsArg0 => IsArg0
        ? _arg0
        : throw new InvalidOperationException("Arg0 value not available");
    public Error[] AsErrors => HasErrors
        ? _errors ?? []
        : throw new InvalidOperationException("Arg1 value not available");

    public Result() {
        throw new InvalidOperationException("Do not use type constructor.");
    }

    private Result(int index, TArg0? arg0 = default, Error[]? errors = null) {
        Index = index;

        _arg0 = arg0;
        _errors = errors;
    }

    public static implicit operator Result<TArg0>(TArg0 arg0)
        => new(0, arg0: arg0);

    public static implicit operator Result<TArg0>(Error error)
        => new(1, errors: [error]);

    public static implicit operator Result<TArg0>(Error[] errors)
        => new(1, errors: errors);

    public void Switch(Action<TArg0?> onArg0, Action<Error[]>? onErrors = null) {
        if (IsArg0) {
            Prevent.Argument.Null(onArg0)(AsArg0);
            return;
        }

        if (HasErrors && onErrors is not null) {
            onErrors(AsErrors);
        }
    }

    public Task Switch(Func<TArg0?, Task> onArg0, Func<Error[], Task>? onErrors = null) {
        if (IsArg0) {
            return Prevent.Argument.Null(onArg0)(AsArg0);
        }

        return HasErrors && onErrors is not null
            ? onErrors(AsErrors)
            : Task.CompletedTask;
    }

    public TResult Match<TResult>(Func<TArg0?, TResult> onArg0, Func<Error[], TResult>? onErrors = null) {
        if (IsArg0) {
            return Prevent.Argument.Null(onArg0)(AsArg0);
        }

        return HasErrors && onErrors is not null
            ? onErrors(AsErrors)
            : default!;
    }
}