namespace Nameless.Results;

public abstract class ResultBase<TArg0, TArg1> : IResult {
    private readonly TArg0? _arg0;
    private readonly TArg1? _arg1;
    private readonly Error[]? _errors;

    public object? Value
        => Index switch {
            0 => _arg0,
            1 => _arg1,
            2 => _errors,
            _ => throw new InvalidOperationException("Invalid index")
        };
    public int Index { get; }
    public bool IsArg0 => Index == 0;
    public bool IsArg1 => Index == 1;
    public bool HasErrors => Index == 2;
    public TArg0? AsArg0 => IsArg0
        ? _arg0
        : throw new InvalidOperationException("Arg0 value not available");
    public TArg1? AsArg1 => IsArg1
        ? _arg1
        : throw new InvalidOperationException("Arg1 value not available");
    public Error[] AsErrors => HasErrors
        ? _errors ?? []
        : throw new InvalidOperationException("Arg1 value not available");

    protected ResultBase() {
        throw new InvalidOperationException("Do not use type parameterless constructor.");
    }

    protected ResultBase(int index, TArg0? arg0 = default, TArg1? arg1 = default, Error[]? errors = null) {
        Index = index;

        _arg0 = arg0;
        _arg1 = arg1;
        _errors = errors;
    }

    public void Switch(Action<TArg0?> onArg0, Action<TArg1?> onArg1, Action<Error[]>? onErrors = null) {
        if (IsArg0) {
            Prevent.Argument.Null(onArg0)(AsArg0);
            return;
        }

        if (IsArg1) {
            Prevent.Argument.Null(onArg1)(AsArg1);
            return;
        }

        if (HasErrors && onErrors is not null) {
            onErrors(AsErrors);
        }
    }

    public Task Switch(Func<TArg0?, Task> onArg0, Func<TArg1?, Task> onArg1, Func<Error[], Task>? onErrors = null) {
        if (IsArg0) {
            return Prevent.Argument.Null(onArg0)(AsArg0);
        }

        if (IsArg1) {
            return Prevent.Argument.Null(onArg1)(AsArg1);
        }

        return HasErrors && onErrors is not null
            ? onErrors(AsErrors)
            : Task.CompletedTask;
    }

    public TResult Match<TResult>(Func<TArg0?, TResult> onArg0, Func<TArg1?, TResult> onArg1, Func<Error[], TResult>? onErrors = null) {
        if (IsArg0) {
            return Prevent.Argument.Null(onArg0)(AsArg0);
        }

        if (IsArg1) {
            return Prevent.Argument.Null(onArg1)(AsArg1);
        }

        return HasErrors && onErrors is not null
            ? onErrors(AsErrors)
            : default!;
    }
}