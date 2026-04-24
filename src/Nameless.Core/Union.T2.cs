using System.Diagnostics.CodeAnalysis;

namespace Nameless;

/// <summary>
///     Represents a value that can be one of several case types.
/// </summary>
/// <typeparam name="TValue0">
///     Type of the first value.
/// </typeparam>
/// <typeparam name="TValue1">
///     Type of the second value.
/// </typeparam>
/// <typeparam name="TValue2">
///     Type of the third value.
/// </typeparam>
public class Union<TValue0, TValue1, TValue2>
{
    private readonly TValue0? _value0;
    private readonly TValue1? _value1;
    private readonly TValue2? _value2;

    /// <summary>
    ///     Whether the current value is of the first type.
    /// </summary>
    [MemberNotNullWhen(returnValue: true, nameof(AsValue0), nameof(_value0))]
    public bool IsValue0 => Index == 0;

    /// <summary>
    ///     Whether the current value is of the second type.
    /// </summary>
    [MemberNotNullWhen(returnValue: true, nameof(AsValue1), nameof(_value1))]
    public bool IsValue1 => Index == 1;

    /// <summary>
    ///     Whether the current value is of the third type.
    /// </summary>
    [MemberNotNullWhen(returnValue: true, nameof(AsValue2), nameof(_value2))]
    public bool IsValue2 => Index == 2;

    /// <summary>
    ///     Gets the value as the first type.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    ///     When result for the first type is not available.
    /// </exception>
    public TValue0 AsValue0 => IsValue0
        ? _value0
        : throw new InvalidOperationException($"Value as '{typeof(TValue0).GetPrettyName()}' is not available");

    /// <summary>
    ///     Gets the value as the second type.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    ///     When result for the second type is not available.
    /// </exception>
    public TValue1 AsValue1 => IsValue1
        ? _value1
        : throw new InvalidOperationException($"Value as '{typeof(TValue1).GetPrettyName()}' is not available");

    /// <summary>
    ///     Gets the value as the third type.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    ///     When result for the third type is not available.
    /// </exception>
    public TValue2 AsValue2 => IsValue2
        ? _value2
        : throw new InvalidOperationException($"Value as '{typeof(TValue2).GetPrettyName()}' is not available");

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
    public Union()
    {
        throw new InvalidOperationException("Do not use the parameterless constructor");
    }

    /// <summary>
    ///     Initializes a new instance of
    ///     <see cref="Union{TValue0,TValue1}"/> class.
    /// </summary>
    /// <param name="index">
    ///     The index of the value.
    /// </param>
    /// <param name="value0">
    ///     The first value.
    /// </param>
    /// <param name="value1">
    ///     The second value.
    /// </param>
    /// <param name="value2">
    ///     The third value.
    /// </param>
    protected Union(int index, TValue0? value0 = default, TValue1? value1 = default, TValue2? value2 = default)
    {
        Index = index;

        _value0 = value0;
        _value1 = value1;
        _value2 = value2;
    }

    /// <summary>
    ///     Gets the current value as object.
    /// </summary>
    public object? Value => Index switch
    {
        0 => _value0,
        1 => _value1,
        2 => _value2,
        _ => throw new InvalidOperationException("Invalid index for value.")
    };

    /// <summary>
    ///     Gets the index of the current value.
    ///     <para>
    ///         <c>0</c> for <typeparamref name="TValue0"/>
    ///     </para>
    ///     <para>
    ///         <c>1</c> for <typeparamref name="TValue1"/>
    ///     </para>
    ///     <para>
    ///         <c>2</c> for <typeparamref name="TValue2"/>
    ///     </para>
    /// </summary>
    public int Index { get; }

    /// <summary>
    ///     Executes the corresponding action given the result.
    /// </summary>
    /// <param name="onValue0">
    ///     Action that will be executed for
    ///     value of <typeparamref name="TValue0"/>.
    /// </param>
    /// <param name="onValue1">
    ///     Action that will be executed for
    ///     value of <typeparamref name="TValue1"/>.
    /// </param>
    /// <param name="onValue2">
    ///     Action that will be executed for
    ///     value of <typeparamref name="TValue2"/>.
    /// </param>
    public void Match(Action<TValue0> onValue0, Action<TValue1> onValue1, Action<TValue2> onValue2)
    {
        if (IsValue0)
        {
            onValue0(AsValue0);

            return;
        }

        if (IsValue1)
        {
            onValue1(AsValue1);

            return;
        }

        if (!IsValue2)
        {
            return;
        }

        onValue2(AsValue2);
    }

    /// <summary>
    ///     Executes the corresponding function given the current value type.
    /// </summary>
    /// <typeparam name="TResult">
    ///     Type of the returned value.
    /// </typeparam>
    /// <param name="onValue0">
    ///     Function that will be executed for the
    ///     value of <typeparamref name="TValue0"/>.
    /// </param>
    /// <param name="onValue1">
    ///     Function that will be executed for the
    ///     value of <typeparamref name="TValue1"/>.
    /// </param>
    /// <param name="onValue2">
    ///     Function that will be executed for the
    ///     value of <typeparamref name="TValue2"/>.
    /// </param>
    /// <returns>
    ///     The result of the corresponding function.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     if there are no argument that matches the functions.
    /// </exception>
    public TResult Match<TResult>(Func<TValue0, TResult> onValue0, Func<TValue1, TResult> onValue1, Func<TValue2, TResult> onValue2)
    {
        if (IsValue0)
        {
            return onValue0(AsValue0);
        }

        if (IsValue1)
        {
            return onValue1(AsValue1);
        }

        if (IsValue2)
        {
            return onValue2(AsValue2);
        }

        throw new InvalidOperationException("Missing union branch.");
    }

    /// <summary>
    ///     Creates a new instance with <paramref name="value0"/> as the
    ///     current result.
    /// </summary>
    /// <param name="value0">
    ///     The result.
    /// </param>
    public static implicit operator Union<TValue0, TValue1, TValue2>(TValue0 value0)
    {
        return new Union<TValue0, TValue1, TValue2>(
            index: 0,
            value0,
            value1: default,
            value2: default
        );
    }

    /// <summary>
    ///     Creates a new instance with <paramref name="value1"/> as the
    ///     current result.
    /// </summary>
    /// <param name="value1">
    ///     The result.
    /// </param>
    public static implicit operator Union<TValue0, TValue1, TValue2>(TValue1 value1)
    {
        return new Union<TValue0, TValue1, TValue2>(
            index: 1,
            value0: default,
            value1,
            value2: default
        );
    }

    /// <summary>
    ///     Creates a new instance with <paramref name="value2"/> as the
    ///     current result.
    /// </summary>
    /// <param name="value2">
    ///     The result.
    /// </param>
    public static implicit operator Union<TValue0, TValue1, TValue2>(TValue2 value2)
    {
        return new Union<TValue0, TValue1, TValue2>(
            index: 2,
            value0: default,
            value1: default,
            value2
        );
    }
}