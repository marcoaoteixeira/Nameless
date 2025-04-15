namespace Nameless.Results;

/// <summary>
/// Represents a result of an operation.
/// </summary>
public interface IResult {
    object? Value { get; }
    int Index { get; }
}

public record Result<TArg0> : Result<TArg0, Error[]> {
    public bool HasErrors
        => IsArg1 && AsArg1?.Length > 0;

    protected Result(int index, TArg0? arg0, Error[] errors)
        : base (index, arg0, errors) { }

    public static implicit operator Result<TArg0>(TArg0 arg0)
        => new(index: 0, arg0: arg0, errors: []);

    public static implicit operator Result<TArg0>(Error error)
        => new(index: 1, arg0: default, errors: [error]);

    public static implicit operator Result<TArg0>(Error[] errors)
        => new(index: 1, arg0: default, errors);
}

public record Result<TArg0, TArg1> : IResult {
    private readonly TArg0? _arg0;
    private readonly TArg1? _arg1;

    public object? Value
        => Index switch {
            0 => _arg0,
            1 => _arg1,
            _ => throw new InvalidOperationException("Invalid index")
        };
    public int Index { get; }
    public bool IsArg0 => Index == 0;
    public bool IsArg1 => Index == 1;

    public TArg0? AsArg0 => IsArg0
        ? _arg0
        : throw new InvalidOperationException("Arg0 value not available");

    public TArg1? AsArg1 => IsArg1
        ? _arg1
        : throw new InvalidOperationException("Arg1 value not available");

    public Result() {
        throw new InvalidOperationException("Do not use type constructor.");
    }

    protected Result(int index, TArg0? arg0 = default, TArg1? arg1 = default) {
        Index = index;

        _arg0 = arg0;
        _arg1 = arg1;
    }

    public static implicit operator Result<TArg0, TArg1>(TArg0 arg0)
        => new(0, arg0: arg0);

    public static implicit operator Result<TArg0, TArg1>(TArg1 arg1)
        => new(1, arg1: arg1);

    public void Switch(Action<TArg0?> onArg0, Action<TArg1?> onArg1) {
        if (IsArg0) {
            Prevent.Argument.Null(onArg0)(AsArg0);
            return;
        }

        if (IsArg1) {
            Prevent.Argument.Null(onArg1)(AsArg1);
        }
    }

    public Task Switch(Func<TArg0?, Task> onArg0, Func<TArg1?, Task> onArg1) {
        if (IsArg0) {
            return Prevent.Argument.Null(onArg0)(AsArg0);
        }

        return IsArg1
            ? Prevent.Argument.Null(onArg1)(AsArg1)
            : Task.CompletedTask;
    }

    public TResult Match<TResult>(Func<TArg0?, TResult> onArg0, Func<TArg1?, TResult> onArg1) {
        if (IsArg0) {
            return Prevent.Argument.Null(onArg0)(AsArg0);
        }

        return IsArg1
            ? Prevent.Argument.Null(onArg1)(AsArg1)
        : default!;
    }
}