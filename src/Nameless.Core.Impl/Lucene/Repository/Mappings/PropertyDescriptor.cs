namespace Nameless.Lucene.Repository.Mappings;

/// <summary>
///     Describes the metadata for a single property mapped to a Lucene document field.
/// </summary>
public abstract record PropertyDescriptor {
    /// <summary>
    ///     Gets the property name, which corresponds to the Lucene field name.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    ///     Gets the CLR type of the property.
    /// </summary>
    public required Type Type { get; init; }

    /// <summary>
    ///     Gets a value indicating whether this property is the entity's unique identifier.
    /// </summary>
    public bool IsID { get; init; }

    /// <summary>
    ///     Gets the storage and indexing options for this field.
    /// </summary>
    public PropertyOptions Options { get; init; }
}

/// <summary>
///     Extends <see cref="PropertyDescriptor"/> with typed getter and setter delegates
///     for efficient property access on <typeparamref name="TDocument"/>.
/// </summary>
/// <typeparam name="TDocument">The document/entity type that owns this property.</typeparam>
public record PropertyDescriptor<TDocument> : PropertyDescriptor
    where TDocument : class {
    /// <summary>
    ///     Gets the compiled delegate used to read this property from an entity instance.
    /// </summary>
    public required Func<TDocument, object?> Getter { get; init; }

    /// <summary>
    ///     Gets the compiled delegate used to write a value to this property on an entity instance.
    /// </summary>
    public required Action<TDocument, object?> Setter { get; init; }
}
