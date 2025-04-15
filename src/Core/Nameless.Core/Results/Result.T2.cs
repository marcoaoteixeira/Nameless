namespace Nameless.Results;

public readonly struct Result<TArg0, TArg1, TArg2> : IResult {
    private readonly TArg0? _arg0;
    private readonly TArg1? _arg1;
    private readonly TArg2? _arg2;
    private readonly Error[]? _errors;

    public object? Value
        => Index switch {
            0 => _arg0,
            1 => _arg1,
            2 => _arg2,
            3 => _errors,
            _ => throw new InvalidOperationException("Invalid index")
        };
    public int Index { get; }
    public bool IsArg0 => Index == 0;
    public bool IsArg1 => Index == 1;
    public bool IsArg2 => Index == 2;
    public bool HasErrors => Index == 3;
    public TArg0? AsArg0 => IsArg0
        ? _arg0
        : throw new InvalidOperationException("Arg0 value not available");
    public TArg1? AsArg1 => IsArg1
        ? _arg1
        : throw new InvalidOperationException("Arg1 value not available");
    public TArg2? AsArg2 => IsArg2
        ? _arg2
        : throw new InvalidOperationException("Arg1 value not available");
    public Error[] AsErrors => HasErrors
        ? _errors ?? []
        : throw new InvalidOperationException("Arg1 value not available");

    public Result() {
        throw new InvalidOperationException("Do not use type constructor.");
    }

    private Result(int index, TArg0? arg0 = default, TArg1? arg1 = default, TArg2? arg2 = default, Error[]? errors = null) {
        Index = index;

        _arg0 = arg0;
        _arg1 = arg1;
        _arg2 = arg2;
        _errors = errors;
    }

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

    public void Switch(Action<TArg0?> onArg0, Action<TArg1?> onArg1, Action<TArg2?> onArg2, Action<Error[]>? onErrors = null) {
        if (IsArg0) {
            Prevent.Argument.Null(onArg0)(AsArg0);
            return;
        }

        if (IsArg1) {
            Prevent.Argument.Null(onArg1)(AsArg1);
            return;
        }

        if (IsArg2) {
            Prevent.Argument.Null(onArg2)(AsArg2);
            return;
        }

        if (HasErrors && onErrors is not null) {
            onErrors(AsErrors);
        }
    }

    public Task Switch(Func<TArg0?, Task> onArg0, Func<TArg1?, Task> onArg1, Func<TArg2?, Task> onArg2, Func<Error[], Task>? onErrors = null) {
        if (IsArg0) {
            return Prevent.Argument.Null(onArg0)(AsArg0);
        }

        if (IsArg1) {
            return Prevent.Argument.Null(onArg1)(AsArg1);
        }

        if (IsArg2) {
            return Prevent.Argument.Null(onArg2)(AsArg2);
        }

        return HasErrors && onErrors is not null
            ? onErrors(AsErrors)
            : Task.CompletedTask;
    }

    public TResult Match<TResult>(Func<TArg0?, TResult> onArg0, Func<TArg1?, TResult> onArg1, Func<TArg2?, TResult> onArg2, Func<Error[], TResult>? onErrors = null) {
        if (IsArg0) {
            return Prevent.Argument.Null(onArg0)(AsArg0);
        }

        if (IsArg1) {
            return Prevent.Argument.Null(onArg1)(AsArg1);
        }

        if (IsArg2) {
            return Prevent.Argument.Null(onArg2)(AsArg2);
        }

        return HasErrors && onErrors is not null
            ? onErrors(AsErrors)
            : default!;
    }
}