#if NET8_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif

namespace Nameless.Result;

/// <summary>
/// Represents a result.
/// </summary>
/// <typeparam name="TValue">The type of the result's value</typeparam>
public sealed record Result<TValue> {
    private readonly Error[] _errors;
    private readonly TValue? _value;

    /// <summary>
    /// Gets the result value.
    /// </summary>
    /// <remarks>
    /// It will throw <see cref="InvalidOperationException"/> if
    /// <see cref="HasErrors"/> is <c>true</c>.
    /// </remarks>
    public TValue Value {
        get {
            if (HasErrors) {
                throw new InvalidOperationException($"The {nameof(Value)} property cannot be accessed when errors have been recorded.");
            }

#pragma warning disable CS8603 // Possible null reference return.
            return _value;
#pragma warning restore CS8603 // Possible null reference return.
        }
    }

    /// <summary>
    /// Returns <c>true</c> if the result has errors;
    /// otherwise <c>false</c>.
    /// </summary>
#if NET8_0_OR_GREATER
    [MemberNotNullWhen(returnValue: false, nameof(_value))]
#endif
    public bool HasErrors => _errors.Length > 0;

    /// <summary>
    /// Gets an array of errors.
    /// </summary>
    /// <remarks>
    /// It will throw <see cref="InvalidOperationException"/> if
    /// <see cref="HasErrors"/> is <c>false</c>.
    /// </remarks>
    public Error[] Errors {
        get {
            if (!HasErrors) {
                throw new InvalidOperationException($"The {nameof(Errors)} property cannot be accessed when no errors have been recorded.");
            }

            return _errors;
        }
    }

    private Result(TValue value) {
        _errors = [];
        _value = value;
    }

    private Result(Error error) {
        _errors = [error];
        _value = default;
    }

    private Result(Error[] errors) {
        _errors = errors;
        _value = default;
    }

    /// <summary>
    /// Implicit converts a value to a <see cref="Result{TValue}"/>
    /// and marks it as succeeded.
    /// </summary>
    /// <param name="value">The value.</param>
    public static implicit operator Result<TValue>(TValue value)
        => new(value);

    /// <summary>
    /// Implicit converts an error to a <see cref="Result{TValue}"/>
    /// and marks it as failed.
    /// </summary>
    /// <param name="error">The error.</param>
    public static implicit operator Result<TValue>(Error error)
        => new(error);

    /// <summary>
    /// Implicit converts an array of errors to a <see cref="Result{TValue}"/>
    /// and marks it as failed.
    /// </summary>
    /// <param name="errors">The array of errors.</param>
    public static implicit operator Result<TValue>(Error[] errors)
        => new(errors);

    /// <summary>
    /// Switches between functions given the result state.
    /// </summary>
    /// <typeparam name="TNext">The match function result.</typeparam>
    /// <param name="onSuccess">Executes this function when <see cref="HasErrors"/> is <c>false</c>.</param>
    /// <param name="onError">Executes this function when <see cref="HasErrors"/> is <c>true</c>.</param>
    /// <returns>
    /// The result value of the matched function.
    /// </returns>
    public TNext Switch<TNext>(Func<TValue, TNext> onSuccess, Func<Error[], TNext> onError)
        => HasErrors ? onError(Errors) : onSuccess(Value);

    public Task<TNext> SwitchAsync<TNext>(Func<TValue, Task<TNext>> onSuccessAsync, Func<Error[], Task<TNext>> onErrorAsync)
        => HasErrors ? onErrorAsync(Errors) : onSuccessAsync(Value);

    public void Switch(Action<TValue> onSuccess, Action<Error[]> onError) {
        if (HasErrors) { onError(Errors); } else { onSuccess(Value); }
    }

    public Task SwitchAsync(Func<TValue, Task> onSuccessAsync, Func<Error[], Task> onErrorAsync)
        => HasErrors ? onErrorAsync(Errors) : onSuccessAsync(Value);
}