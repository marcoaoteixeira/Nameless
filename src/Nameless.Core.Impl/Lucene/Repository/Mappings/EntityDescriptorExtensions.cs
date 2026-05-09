using System.Linq.Expressions;

namespace Nameless.Lucene.Repository.Mappings;

/// <summary>
///     <see cref="IEntityDescriptor{TEntity}"/> extension methods.
/// </summary>
public static class EntityDescriptorExtensions {
    /// <param name="self">The current <see cref="IEntityDescriptor{TEntity}"/> instance.</param>
    extension<TEntity>(IEntityDescriptor<TEntity> self) where TEntity : class {
        /// <summary>
        ///     Registers a property with the default <see cref="PropertyOptions.Store"/> option.
        /// </summary>
        /// <typeparam name="TProperty">The property type.</typeparam>
        /// <param name="expression">A member expression pointing to the property.</param>
        /// <returns>The current <see cref="IEntityDescriptor{TEntity}"/> for chaining.</returns>
        public IEntityDescriptor<TEntity> Property<TProperty>(Expression<Func<TEntity, TProperty>> expression) {
            return self.SetProperty(expression, PropertyOptions.Store);
        }
    }
}