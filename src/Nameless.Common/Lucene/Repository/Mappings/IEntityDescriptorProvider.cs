namespace Nameless.Lucene.Repository.Mappings;

/// <summary>
///     Provides <see cref="IEntityDescriptor{TEntity}"/> instances for entity types,
///     using registered <see cref="IEntityMapping{TEntity}"/> implementations.
/// </summary>
public interface IEntityDescriptorProvider {
    /// <summary>
    ///     Retrieves the <see cref="IEntityDescriptor{TEntity}"/> for the specified entity type,
    ///     building and caching it from the mapping if not already present.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <returns>The <see cref="IEntityDescriptor{TEntity}"/> for <typeparamref name="TEntity"/>.</returns>
    /// <exception cref="MissingEntityIDException">
    ///     if the entity mapping does not define an ID property.
    /// </exception>
    IEntityDescriptor<TEntity> GetDescriptor<TEntity>() where TEntity : class;
}
