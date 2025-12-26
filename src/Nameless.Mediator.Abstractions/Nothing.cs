#pragma warning disable IDE0060 // Remove unused parameter

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Nameless.Mediator;

/// <summary>
///     Represents a void type, since <see cref="Void" /> is not a valid return type in C#.
/// </summary>
[DebuggerDisplay(value: "{DebuggerDisplayValue,nq}")]
public readonly struct Nothing : IEquatable<Nothing>,
    IComparable<Nothing>,
    IComparable {
    private static readonly Nothing InnerValue = new();

    /// <summary>
    ///     Default and only value of the <see cref="Nothing" /> type.
    /// </summary>
    public static ref readonly Nothing Value => ref InnerValue;

    private static string DebuggerDisplayValue => nameof(Nothing);

    /// <summary>
    /// Private constructor to prevent external instantiation.
    /// Only the static Value property can create instances.
    /// </summary>
    static Nothing() {
        // Static constructor ensures InnerValue is initialized
    }

    /// <summary>
    /// Determines whether two Nothing instances are equal.
    /// </summary>
    /// <param name="left">The first Nothing instance to compare.</param>
    /// <param name="right">The second Nothing instance to compare.</param>
    /// <returns>Always returns true.</returns>
    public static bool operator ==(Nothing left, Nothing right) {
        return true;
    }

    /// <summary>
    /// Determines whether two Nothing instances are not equal.
    /// </summary>
    /// <param name="left">The first Nothing instance to compare.</param>
    /// <param name="right">The second Nothing instance to compare.</param>
    /// <returns>Always returns false.</returns>
    public static bool operator !=(Nothing left, Nothing right) {
        return false;
    }

    /// <summary>
    /// Determines whether one Nothing instance is less than another.
    /// </summary>
    /// <param name="left">The first Nothing instance to compare.</param>
    /// <param name="right">The second Nothing instance to compare.</param>
    /// <returns>Always returns false, as all Nothing instances are equal.</returns>
    public static bool operator <(Nothing left, Nothing right) {
        return false;
    }

    /// <summary>
    /// Determines whether one Nothing instance is greater than another.
    /// </summary>
    /// <param name="left">The first Nothing instance to compare.</param>
    /// <param name="right">The second Nothing instance to compare.</param>
    /// <returns>Always returns false, as all Nothing instances are equal.</returns>
    public static bool operator >(Nothing left, Nothing right) {
        return false;
    }

    /// <summary>
    /// Determines whether one Nothing instance is less than or equal to another.
    /// </summary>
    /// <param name="left">The first Nothing instance to compare.</param>
    /// <param name="right">The second Nothing instance to compare.</param>
    /// <returns>Always returns true, as all Nothing instances are equal.</returns>
    public static bool operator <=(Nothing left, Nothing right) {
        return true;
    }

    /// <summary>
    /// Determines whether one Nothing instance is greater than or equal to another.
    /// </summary>
    /// <param name="left">The first Nothing instance to compare.</param>
    /// <param name="right">The second Nothing instance to compare.</param>
    /// <returns>Always returns true, as all Nothing instances are equal.</returns>
    public static bool operator >=(Nothing left, Nothing right) {
        return true;
    }

    /// <summary>
    /// Executes an action and returns Nothing, useful for chaining operations.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <returns>Nothing.Value</returns>
    public static Nothing Then(Action action) {
        action.Invoke();

        return Value;
    }

    /// <summary>
    ///     Compares the current object with another object of the same type.
    /// </summary>
    /// <param name="other">An object to compare with this object.</param>
    /// <returns>
    ///     A value that indicates the relative order of the objects being compared.
    ///     The return value has the following meanings:
    ///     - Less than zero: This object is less than the <paramref name="other" /> parameter.
    ///     - Zero: This object is equal to <paramref name="other" />.
    ///     - Greater than zero: This object is greater than <paramref name="other" />.
    /// </returns>
    public int CompareTo(Nothing other) {
        return 0;
    }

    /// <summary>
    ///     Compares the current instance with another object of the same type and returns an integer that indicates whether
    ///     the current instance precedes, follows, or occurs in the same position in the sort order as the obj object.
    /// </summary>
    /// <param name="obj">An object to compare with this instance.</param>
    /// <returns>
    ///     A value that indicates the relative order of the objects being compared.
    ///     The return value has these meanings:
    ///     - Less than zero: This instance precedes <paramref name="obj" /> in the sort order.
    ///     - Zero: This instance occurs in the same position in the sort order as <paramref name="obj" />.
    ///     - Greater than zero: This instance follows <paramref name="obj" /> in the sort order.
    /// </returns>
    int IComparable.CompareTo(object? obj) {
        if (obj is null) {
            return 1;
        }

        if (obj is not Nothing) {
            throw new ArgumentException($"Parameter is not type '{nameof(Nothing)}'.", nameof(obj));
        }

        return 0;
    }

    /// <summary>
    ///     Returns a hash code for this instance.
    /// </summary>
    /// <returns>
    ///     A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
    /// </returns>
    public override int GetHashCode() {
        return 0;
    }

    /// <summary>
    ///     Determines whether the current object is equal to another object of the same type.
    /// </summary>
    /// <param name="other">An object to compare with this object.</param>
    /// <returns>
    ///     <see langword="true"/> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <see langword="false"/>.
    /// </returns>
    public bool Equals(Nothing other) {
        return true;
    }

    /// <summary>
    ///     Determines whether the specified <see cref="object" /> is equal to this instance.
    /// </summary>
    /// <param name="obj">The object to compare with the current instance.</param>
    /// <returns>
    ///     <see langword="true"/> if the specified <see cref="object" /> is equal to this instance; otherwise, <see langword="false"/>.
    /// </returns>
    public override bool Equals([NotNullWhen(returnValue: true)] object? obj) {
        return obj is Nothing;
    }

    /// <summary>
    ///     Returns a <see cref="string" /> that represents this instance.
    /// </summary>
    /// <returns>A <see cref="string" /> that represents this instance.</returns>
    public override string ToString() {
        return "()";
    }
}