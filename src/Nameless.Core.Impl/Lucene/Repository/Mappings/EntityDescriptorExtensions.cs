using System.Linq.Expressions;

namespace Nameless.Lucene.Repository.Mappings;

public static class EntityDescriptorExtensions {
    extension<TEntity>(IEntityDescriptor<TEntity> self) where TEntity : class {
        public IEntityDescriptor<TEntity> Property<TProperty>(Expression<Func<TEntity, TProperty>> expression) {
            return self.SetProperty(expression, PropertyOptions.Store);
        }
    }
}