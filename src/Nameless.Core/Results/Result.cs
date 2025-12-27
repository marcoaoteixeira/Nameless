using System.Diagnostics.CodeAnalysis;
using Nameless.ObjectModel;

namespace Nameless.Results;

/// <summary>
///     Represents a result or a list of errors.
/// </summary>
/// <typeparam name="T">
///     Type of the result.
/// </typeparam>
public class Result<T> {
    private readonly object? _value;
    private readonly Error[] _errors;

    /// <summary>
    ///     Indicates if there is a valid result value.
    /// </summary>
    [MemberNotNullWhen(returnValue: true, nameof(Value), nameof(_value))]
    public bool Success => _errors.Length == 0;

    /// <summary>
    ///     Gets the result.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    ///     When result is not available.
    /// </exception>
    public T Value => Success
        ? (T)_value
        : throw new InvalidOperationException(message: "Result is not available");

    /// <summary>
    ///     Gets the errors.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    ///     When errors are not available.
    /// </exception>
    public Error[] Errors => !Success
        ? _errors
        : throw new InvalidOperationException(message: "Errors are not available");

    // Keep unused public constructor to avoid the use of parameterless
    // constructor.
    public Result() {
        throw new InvalidOperationException(message: "Do not use the parameterless constructor");
    }

    /// <summary>
    ///     Initializes a new instance of
    ///     <see cref="Result{TResult}"/> class.
    /// </summary>
    /// <param name="value">
    ///     The result.
    /// </param>
    /// <param name="errors">
    ///     The errors.
    /// </param>
    protected Result(object? value, Error[] errors) {
        _value = value;
        _errors = errors;
    }

    /// <summary>
    ///     Executes the corresponding action given the result.
    /// </summary>
    /// <param name="onSuccess">
    ///     Action that will be executed on <see cref="T"/>.
    /// </param>
    /// <param name="onFailure">
    ///     Action that will be executed on errors.
    /// </param>
    public void Match(Action<T> onSuccess, Action<Error[]> onFailure) {
        if (Success) {
            onSuccess(Value);

            return;
        }

        onFailure(Errors);
    }

    /// <summary>
    ///     Executes the corresponding function given the value of the result.
    /// </summary>
    /// <param name="onSuccess">
    ///     Function that will be executed on <see cref="T"/> result.
    /// </param>
    /// <param name="onFailure">
    ///     Function that will be executed on errors.
    /// </param>
    public TReturnValue Match<TReturnValue>(Func<T, TReturnValue> onSuccess, Func<Error[], TReturnValue> onFailure) {
        return Success
            ? onSuccess(Value)
            : onFailure(Errors);
    }

    /// <summary>
    ///     Creates a new instance with the specified result.
    /// </summary>
    /// <param name="result">
    ///     The result.
    /// </param>
    public static implicit operator Result<T>(T result) {
        return new Result<T>(result, []);
    }

    /// <summary>
    ///     Creates a new instance with the specified error.
    /// </summary>
    /// <param name="error">
    ///     The error.
    /// </param>
    public static implicit operator Result<T>(Error error) {
        return new Result<T>(value: null, [error]);
    }

    /// <summary>
    ///     Creates a new instance with the specified errors.
    /// </summary>
    /// <param name="errors">
    ///     The errors.
    /// </param>
    public static implicit operator Result<T>(Error[] errors) {
        return new Result<T>(value: null, errors);
    }
}