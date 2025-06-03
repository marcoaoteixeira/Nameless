namespace Nameless.Results;

/// <summary>
/// Base class for results with one argument of type <typeparamref name="TArg0"/> or an array of errors.
/// </summary>
/// <typeparam name="TArg0">Type of the argument at position 0.</typeparam>
public abstract class ResultBase<TArg0> : IResult {
    private readonly TArg0? _arg0;
    private readonly Error[]? _errors;

    /// <summary>
    /// Indicates whether the result has a value for the argument at position 0.
    /// </summary>
    public bool IsArg0 => Index == 0;

    /// <summary>
    /// Indicates whether the result has errors.
    /// </summary>
    public bool HasErrors => Index == 1;

    /// <summary>
    /// Gets the argument at position 0.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// If argument at position 0 is unavailable.
    /// </exception>
    public TArg0? AsArg0 => IsArg0
        ? _arg0
        : throw new InvalidOperationException("Arg0 value not available");

    /// <summary>
    /// Gets the errors.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// If errors are unavailable.
    /// </exception>
    public Error[] AsErrors => HasErrors
        ? _errors ?? []
        : throw new InvalidOperationException("Arg1 value not available");

    /// <summary>
    /// Do not use this constructor.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// If constructor is called.
    /// </exception>
    protected ResultBase() {
        throw new InvalidOperationException("Do not use type parameterless constructor.");
    }

    /// <summary>
    /// Construct <see cref="ResultBase{TArg0}"/> base type.
    /// </summary>
    /// <param name="index">The index of the argument.</param>
    /// <param name="arg0">The argument at position 0.</param>
    /// <param name="errors">The errors.</param>
    protected ResultBase(int index, TArg0? arg0 = default, Error[]? errors = null) {
        Index = index;

        _arg0 = arg0;
        _errors = errors;
    }

    /// <inheritdoc />
    public object? Value
        => Index switch {
            0 => _arg0,
            1 => _errors,
            _ => throw new InvalidOperationException("Invalid index")
        };

    /// <inheritdoc />
    public int Index { get; }

    /// <summary>
    /// Executes the corresponding action given the value of the result.
    /// </summary>
    /// <param name="onArg0">Action for argument at position 0.</param>
    /// <param name="onErrors">Action for errors.</param>
    public void Switch(Action<TArg0?> onArg0, Action<Error[]> onErrors) {
        if (IsArg0) {
            onArg0(AsArg0);

            return;
        }

        if (HasErrors) {
            onErrors(AsErrors);
        }
    }

    /// <summary>
    /// Executes the corresponding function given the value of the result.
    /// </summary>
    /// <param name="onArg0">Function for argument at position 0.</param>
    /// <param name="onErrors">Function for errors.</param>
    /// <returns>
    /// An asynchronous task representing the execution.
    /// </returns>
    public Task Switch(Func<TArg0?, Task> onArg0, Func<Error[], Task> onErrors) {
        if (IsArg0) {
            return onArg0(AsArg0);
        }

        return HasErrors
            ? onErrors(AsErrors)
            : Task.CompletedTask;
    }

    /// <summary>
    /// Executes the corresponding function given the value of the result
    /// and returns the result of the function.
    /// </summary>
    /// <typeparam name="TResult">Type of the result.</typeparam>
    /// <param name="onArg0">Function for argument at position 0.</param>
    /// <param name="onErrors">Function for errors.</param>
    /// <returns>
    /// An asynchronous task representing the execution.
    /// </returns>
    public TResult Match<TResult>(Func<TArg0?, TResult> onArg0, Func<Error[], TResult> onErrors) {
        if (IsArg0) {
            return onArg0(AsArg0);
        }

        return HasErrors
            ? onErrors(AsErrors)
            : default!;
    }
}