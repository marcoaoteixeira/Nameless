using System.Diagnostics.CodeAnalysis;

namespace Nameless.Results;

/// <summary>
///     Base class for results.
/// </summary>
/// <typeparam name="TResult">
///     Type of the result.
/// </typeparam>
public abstract class ResultBase<TResult> : IResult {
    private readonly TResult? _result;
    private readonly Error _error;

    /// <summary>
    ///     Whether the result is an actual result and not an error.
    /// </summary>
    [MemberNotNullWhen(returnValue: true, nameof(AsResult))]
    public bool IsResult => Index == 0;

    /// <summary>
    ///     Whether the result is an actual error.
    /// </summary>
    public bool IsError => Index == 1;

    /// <summary>
    ///     Gets the result.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    ///     When result is not available.
    /// </exception>
    public TResult? AsResult => IsResult
        ? _result
        : throw new InvalidOperationException("Result is not available");

    /// <summary>
    ///     Gets the error.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    ///     When error is not available.
    /// </exception>
    public Error AsError => IsError
        ? _error
        : throw new InvalidOperationException("Error is not available");

    /// <summary>
    ///     Do not use this constructor.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    ///     If constructor is called.
    /// </exception>
    protected ResultBase() {
        throw new InvalidOperationException("Do not use type parameterless constructor.");
    }

    /// <summary>
    ///     Initializes a new instance of
    ///     <see cref="ResultBase{TResult}"/> class.
    /// </summary>
    /// <param name="index">
    ///     The index of the argument.
    /// </param>
    /// <param name="result">
    ///     The result.
    /// </param>
    /// <param name="error">
    ///     The error.
    /// </param>
    protected ResultBase(int index, TResult? result = default, Error error = default) {
        Index = index;

        _result = result;
        _error = error;
    }

    /// <inheritdoc />
    public object? Value => Index switch {
        0 => _result,
        1 => _error,
        _ => throw new InvalidOperationException("Invalid index for value.")
    };

    /// <inheritdoc />
    public int Index { get; }

    /// <summary>
    ///     Executes the corresponding action given the value of the result.
    /// </summary>
    /// <param name="onResult">
    ///     Action that will be executed if the result is <see cref="TResult"/>.
    /// </param>
    /// <param name="onError">
    ///     Action that will be executed if the result is <see cref="Error"/>.
    /// </param>
    public void Switch(Action<TResult> onResult, Action<Error> onError) {
        if (IsResult) {
            onResult(AsResult);

            return;
        }

        if (IsError) {
            onError(AsError);
        }
    }

    /// <summary>
    ///     Executes the corresponding function given the value of the result.
    /// </summary>
    /// <param name="onResult">
    ///     Function that will be executed if the result is
    ///     <see cref="TResult"/>.
    /// </param>
    /// <param name="onError">
    ///     Function that will be executed if the result is
    ///     <see cref="Error"/>.
    /// </param>
    /// <returns>
    ///     An asynchronous task representing the execution.
    /// </returns>
    public Task Switch(Func<TResult, Task> onResult, Func<Error, Task> onError) {
        if (IsResult) {
            return onResult(AsResult);
        }

        if (IsError) {
            return onError(AsError);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    ///     Executes the corresponding function given the value of the result,
    ///     and returns the result of the function.
    /// </summary>
    /// <typeparam name="TReturnValue">
    ///     Type of the returned value.
    /// </typeparam>
    /// <param name="onResult">
    ///     Function that will be executed if the result is
    ///     <see cref="TResult"/>.
    /// </param>
    /// <param name="onError">
    ///     Function that will be executed if the result is
    ///     <see cref="Error"/>.
    /// </param>
    /// <returns>
    ///     An asynchronous task representing the execution, where the result
    ///     of the function is <see cref="TResult"/>.
    /// </returns>
    public TReturnValue Match<TReturnValue>(Func<TResult, TReturnValue> onResult, Func<Error, TReturnValue> onError) {
        if (IsResult) {
            return onResult(AsResult);
        }

        if (IsError) {
            return onError(AsError!);
        }

        return default!;
    }
}