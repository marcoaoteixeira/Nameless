#pragma warning disable S3875
#nullable disable

namespace Nameless.Barebones.Domains.Common;

/// <summary>
/// Represents a base class for value objects.
/// </summary>
/// <remarks>
/// Learn more: <a href="https://docs.microsoft.com/en-us/dotnet/standard/microservices-architecture/microservice-ddd-cqrs-patterns/implement-value-objects">Implement value objects</a>
/// </remarks>
public abstract class ValueObject {
    /// <summary>
    /// Compares value objects for equality.
    /// </summary>
    /// <param name="left">The left side.</param>
    /// <param name="right">The right side.</param>
    /// <returns>
    /// <see langword="true"/> if the value objects are equal; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool operator ==(ValueObject left, ValueObject right) => EqualOperator(left, right);

    /// <summary>
    /// Compares value objects for inequality.
    /// </summary>
    /// <param name="left">The left side.</param>
    /// <param name="right">The right side.</param>
    /// <returns>
    /// <see langword="true"/> if the value objects are not equal; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool operator !=(ValueObject left, ValueObject right) => NotEqualOperator(left, right);

    /// <summary>
    /// Compares two value objects for equality.
    /// </summary>
    /// <param name="left">The left side.</param>
    /// <param name="right">The right side.</param>
    /// <returns>
    /// <see langword="true"/> if the value objects are equal; otherwise, <see langword="false"/>.
    /// </returns>
    protected static bool EqualOperator(ValueObject left, ValueObject right) {
        if (left is null ^ right is null) {
            return false;
        }

        return ReferenceEquals(left, right) || left.Equals(right);
    }

    /// <summary>
    /// Compares value objects for inequality.
    /// </summary>
    /// <param name="left">The left side.</param>
    /// <param name="right">The right side.</param>
    /// <returns>
    /// <see langword="true"/> if the value objects are not equal; otherwise, <see langword="false"/>.
    /// </returns>
    protected static bool NotEqualOperator(ValueObject left, ValueObject right) {
        return !EqualOperator(left, right);
    }

    /// <summary>
    /// Gets the value object components that are used for equality comparison.
    /// </summary>
    /// <returns>
    /// An <see cref="IEnumerable{T}"/> of objects that represent the value object components.
    /// </returns>
    protected abstract IEnumerable<object> GetEqualityComponents();

    /// <inheritdoc />
    public override bool Equals(object obj) {
        if (obj is null || obj.GetType() != GetType()) {
            return false;
        }

        var other = (ValueObject)obj;
        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    /// <inheritdoc />
    public override int GetHashCode() {
        var hash = new HashCode();

        foreach (var component in GetEqualityComponents()) {
            hash.Add(component);
        }

        return hash.ToHashCode();
    }
}