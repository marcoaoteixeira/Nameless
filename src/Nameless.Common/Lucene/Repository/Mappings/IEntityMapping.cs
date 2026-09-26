namespace Nameless.Lucene.Repository.Mappings;

/// <summary>
///     Defines how the properties of <typeparamref name="TEntity"/> are mapped to
///     and from a Lucene document using an <see cref="IEntityDescriptor{TEntity}"/>.
/// </summary>
/// <typeparam name="TEntity">The entity type to map.</typeparam>
public interface IEntityMapping<TEntity> where TEntity : class {
    /// <summary>
    ///     Configures the property descriptors for <typeparamref name="TEntity"/>
    ///     on the given <paramref name="descriptor"/>.
    /// </summary>
    /// <param name="descriptor">The descriptor to configure.</param>
    void Map(IEntityDescriptor<TEntity> descriptor);
}