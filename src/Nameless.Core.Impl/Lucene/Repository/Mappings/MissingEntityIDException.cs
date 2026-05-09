namespace Nameless.Lucene.Repository.Mappings;

/// <summary>
///     Thrown when an entity mapping does not define an ID property descriptor,
///     which is required for Lucene document identity.
/// </summary>
public class MissingEntityIDException : Exception {
    /// <summary>
    ///     Initializes a new <see cref="MissingEntityIDException"/> for the specified entity type.
    /// </summary>
    /// <param name="type">The entity type that is missing its ID property descriptor.</param>
    public MissingEntityIDException(Type type)
        : base ($"Entity type '{type.GetPrettyName()}' is missing its ID property descriptor.") { }
}
