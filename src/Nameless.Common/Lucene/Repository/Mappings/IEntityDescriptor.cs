using System.Linq.Expressions;

namespace Nameless.Lucene.Repository.Mappings;

/// <summary>
///     Describes how properties of <typeparamref name="TEntity"/> are mapped to and from a
///     Lucene <see cref="global::Lucene.Net.Documents.Document"/>.
/// </summary>
/// <typeparam name="TEntity">The entity type being described.</typeparam>
public interface IEntityDescriptor<TEntity> where TEntity : class {
    /// <summary>
    ///     Gets the collection of property descriptors defined for this entity.
    /// </summary>
    IReadOnlyCollection<PropertyDescriptor> Properties { get; }

    /// <summary>
    ///     Designates the given property as the entity's unique identifier in the index.
    /// </summary>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <param name="expression">A member expression pointing to the ID property.</param>
    /// <returns>The current <see cref="IEntityDescriptor{TEntity}"/> for chaining.</returns>
    /// <exception cref="InvalidOperationException">
    ///     if an ID property has already been defined.
    /// </exception>
    IEntityDescriptor<TEntity> SetID<TProperty>(Expression<Func<TEntity, TProperty>> expression);

    /// <summary>
    ///     Registers a regular indexable property with the given storage options.
    /// </summary>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <param name="expression">A member expression pointing to the property.</param>
    /// <param name="options">The <see cref="PropertyOptions"/> controlling how the field is stored and indexed.</param>
    /// <returns>The current <see cref="IEntityDescriptor{TEntity}"/> for chaining.</returns>
    IEntityDescriptor<TEntity> SetProperty<TProperty>(Expression<Func<TEntity, TProperty>> expression, PropertyOptions options);
}