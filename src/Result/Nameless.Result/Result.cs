﻿#if NET8_0_OR_GREATER
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
    /// <see cref="Succeeded"/> is <c>false</c>.
    /// </remarks>
    public TValue Value {
        get {
            if (!Succeeded) {
                throw new InvalidOperationException($"The {nameof(Value)} property cannot be accessed when errors have been recorded.");
            }

            return _value ?? throw new ArgumentNullException(nameof(_value));
        }
    }

    /// <summary>
    /// Returns <c>true</c> if the result is marked as successful;
    /// otherwise <c>false</c>.
    /// </summary>
#if NET8_0_OR_GREATER
    [MemberNotNullWhen(returnValue: true, nameof(_value))]
#endif
    public bool Succeeded => _errors.Length == 0;

    /// <summary>
    /// Gets an array of errors.
    /// </summary>
    /// <remarks>
    /// It will throw <see cref="InvalidOperationException"/> if
    /// <see cref="Succeeded"/> is <c>true</c>.
    /// </remarks>
    public Error[] Errors {
        get {
            if (Succeeded) {
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
    /// Matches the specified functions given the result state.
    /// </summary>
    /// <typeparam name="TNext">The match function result.</typeparam>
    /// <param name="onSuccess">Executes this function when <see cref="Succeeded"/> is <c>true</c>.</param>
    /// <param name="onFailure">Executes this function when <see cref="Succeeded"/> is <c>false</c>.</param>
    /// <returns>
    /// The result value of the matched function.
    /// </returns>
    public TNext Match<TNext>(Func<TValue, TNext> onSuccess, Func<Error[], TNext> onFailure)
        => Succeeded ? onSuccess(Value) : onFailure(Errors);

    public Task<TNext> MatchAsync<TNext>(Func<TValue, Task<TNext>> onSuccess, Func<Error[], Task<TNext>> onFailure)
        => Succeeded ? onSuccess(Value) : onFailure(Errors);

    public void Match(Action<TValue> onSuccess, Action<Error[]> onFailure) {
        if (Succeeded) { onSuccess(Value); }
        else { onFailure(Errors); }
    }

    public Task MatchAsync(Func<TValue, Task> onSuccess, Func<Error[], Task> onFailure)
        => Succeeded ? onSuccess(Value) : onFailure(Errors);
}