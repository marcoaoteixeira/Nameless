using System.Diagnostics.CodeAnalysis;

namespace Nameless.Results;

/// <summary>
///     Represents a return value that can switch between results.
/// </summary>
/// <typeparam name="TArg0">
///     Type of the first result.
/// </typeparam>
/// <typeparam name="TArg1">
///     Type of the second result.
/// </typeparam>
public class Switch<TArg0, TArg1>
{
    private readonly TArg0? _arg0;
    private readonly TArg1? _arg1;

    /// <summary>
    ///     Whether the actual result is the first.
    /// </summary>
    [MemberNotNullWhen(returnValue: true, nameof(AsArg0), nameof(_arg0))]
    public bool IsArg0 => Index == 0;

    /// <summary>
    ///     Whether the actual result is the second.
    /// </summary>
    [MemberNotNullWhen(returnValue: true, nameof(AsArg1), nameof(_arg1))]
    public bool IsArg1 => Index == 1;

    /// <summary>
    ///     Gets the result as the first type.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    ///     When result for the first type is not available.
    /// </exception>
    public TArg0 AsArg0 => IsArg0
        ? _arg0
        : throw new InvalidOperationException("Arg0 is not available");

    /// <summary>
    ///     Gets the result as the second type.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    ///     When result for the first second is not available.
    /// </exception>
    public TArg1 AsArg1 => IsArg1
        ? _arg1
        : throw new InvalidOperationException("Arg1 is not available");

    /// <summary>
    ///     This constructor is not intended for use and will always throw
    ///     an exception.
    /// </summary>
    /// <remarks>
    ///     This constructor is provided to prevent instantiation without
    ///     parameters, ensuring that the Switch class is always initialized
    ///     with the required parameters.
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when attempting to use the parameterless constructor,
    ///     indicating that it should not be used.
    /// </exception>
    public Switch()
    {
        throw new InvalidOperationException("Do not use the parameterless constructor");
    }

    /// <summary>
    ///     Initializes a new instance of
    ///     <see cref="Switch{TArg0, TArg1}"/> class.
    /// </summary>
    /// <param name="index">
    ///     The index of the argument.
    /// </param>
    /// <param name="arg0">
    ///     The first argument.
    /// </param>
    /// <param name="arg1">
    ///     The second argument.
    /// </param>
    protected Switch(int index, TArg0? arg0 = default, TArg1? arg1 = default)
    {
        Index = index;

        _arg0 = arg0;
        _arg1 = arg1;
    }

    /// <summary>
    ///     Gets the value for the current result.
    /// </summary>
    public object? Value => Index switch
    {
        0 => _arg0,
        1 => _arg1,
        _ => throw new InvalidOperationException("Invalid index for value.")
    };

    /// <summary>
    ///     Gets the index of the current result.
    ///     E.g., 0 for <typeparamref name="TArg0"/>, 1 for <typeparamref name="TArg1"/>, etc.
    /// </summary>
    public int Index { get; }

    /// <summary>
    ///     Executes the corresponding action given the result.
    /// </summary>
    /// <param name="onArg0">
    ///     Action that will be executed for the argument of <typeparamref name="TArg0"/>.
    /// </param>
    /// <param name="onArg1">
    ///     Action that will be executed for the argument of <typeparamref name="TArg1"/>.
    /// </param>
    public void Match(Action<TArg0> onArg0, Action<TArg1> onArg1)
    {
        if (IsArg0)
        {
            onArg0(AsArg0);

            return;
        }

        if (!IsArg1)
        {
            return;
        }

        onArg1(AsArg1);
    }

    /// <summary>
    ///     Executes the corresponding function given the result.
    /// </summary>
    /// <typeparam name="TResult">
    ///     Type of the returned value.
    /// </typeparam>
    /// <param name="onArg0">
    ///     Function that will be executed for the
    ///     argument <typeparamref name="TArg0"/>.
    /// </param>
    /// <param name="onArg1">
    ///     Function that will be executed for the
    ///     argument <typeparamref name="TArg1"/>.
    /// </param>
    /// <returns>
    ///     The result of the corresponding function.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     if there are no argument that matches the functions.
    /// </exception>
    public TResult Match<TResult>(Func<TArg0, TResult> onArg0, Func<TArg1, TResult> onArg1)
    {
        if (IsArg0)
        {
            return onArg0(AsArg0);
        }

        if (IsArg1)
        {
            return onArg1(AsArg1);
        }

        throw new InvalidOperationException("Missing switch branch.");
    }

    /// <summary>
    ///     Creates a new instance with <paramref name="arg0"/> as the
    ///     current result.
    /// </summary>
    /// <param name="arg0">
    ///     The result.
    /// </param>
    public static implicit operator Switch<TArg0, TArg1>(TArg0 arg0)
    {
        return new Switch<TArg0, TArg1>(index: 0, arg0, arg1: default);
    }

    /// <summary>
    ///     Creates a new instance with <paramref name="arg1"/> as the
    ///     current result.
    /// </summary>
    /// <param name="arg1">
    ///     The result.
    /// </param>
    public static implicit operator Switch<TArg0, TArg1>(TArg1 arg1)
    {
        return new Switch<TArg0, TArg1>(index: 1, arg0: default, arg1);
    }
}