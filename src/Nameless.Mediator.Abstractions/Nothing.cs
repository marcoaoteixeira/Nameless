using System.Diagnostics;

namespace Nameless.Mediator;

/// <summary>
///     Represents a void type, since <see cref="System.Void" /> is not a valid return type in C#.
/// </summary>
[DebuggerDisplay("{DebuggerDisplayValue}")]
public readonly struct Nothing : IEquatable<Nothing>,
                                 IComparable<Nothing>,
                                 IComparable {
    private static readonly Nothing InnerValue = new();

    /// <summary>
    ///     Default and only value of the <see cref="Nothing" /> type.
    /// </summary>
    public static ref readonly Nothing Value => ref InnerValue;

    private string DebuggerDisplayValue => nameof(Nothing);

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
    ///     the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
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
    ///     <c>true</c> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <c>false</c>.
    /// </returns>
    public bool Equals(Nothing other) {
        return true;
    }

    /// <summary>
    ///     Determines whether the specified <see cref="System.Object" /> is equal to this instance.
    /// </summary>
    /// <param name="obj">The object to compare with the current instance.</param>
    /// <returns>
    ///     <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
    /// </returns>
    public override bool Equals(object? obj) {
        return obj is Nothing;
    }

    public static bool operator ==(Nothing _, Nothing __) => true;

    public static bool operator !=(Nothing _, Nothing __) => false;

    /// <summary>
    ///     Returns a <see cref="System.String" /> that represents this instance.
    /// </summary>
    /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
    public override string ToString() {
        return "()";
    }
}