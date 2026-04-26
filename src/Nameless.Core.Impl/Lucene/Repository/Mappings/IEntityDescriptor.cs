using System.Linq.Expressions;

namespace Nameless.Lucene.Repository.Mappings;

public interface IEntityDescriptor<TEntity> where TEntity : class {
    IReadOnlyCollection<PropertyDescriptor> Properties { get; }

    IEntityDescriptor<TEntity> SetID<TProperty>(Expression<Func<TEntity, TProperty>> expression);
    
    IEntityDescriptor<TEntity> SetProperty<TProperty>(Expression<Func<TEntity, TProperty>> expression, PropertyOptions options);
}