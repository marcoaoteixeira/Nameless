namespace Nameless.MongoDB.Infrastructure;

/// <summary>
/// Defines a name for a MongoDB collection.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class CollectionNameAttribute : Attribute {
    /// <summary>
    /// Gets the name of the MongoDB collection.
    /// </summary>
    public string Name { get; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CollectionNameAttribute"/>.
    /// </summary>
    /// <param name="name">The collection name.</param>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="name"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     Thrown when <paramref name="name"/> is empty or white space.
    /// </exception>
    public CollectionNameAttribute(string name) {
        Name = Guard.Against.NullOrWhiteSpace(name);
    }
}