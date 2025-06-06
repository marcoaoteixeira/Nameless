namespace Nameless.MongoDB;

/// <summary>
/// Defines a strategy for naming MongoDB collections based on the type of the entity.
/// </summary>
public interface ICollectionNamingStrategy {
    /// <summary>
    /// Gets the name of the MongoDB collection for a given type.
    /// </summary>
    /// <param name="type">Type of the collection.</param>
    /// <returns>
    /// The name of the MongoDB collection as a string.
    /// </returns>
    string GetCollectionName(Type type);
}