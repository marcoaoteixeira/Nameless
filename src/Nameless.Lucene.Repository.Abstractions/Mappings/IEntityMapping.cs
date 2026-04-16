namespace Nameless.Lucene.Repository.Mappings;

public interface IEntityMapping<TEntity> where TEntity : class {
    void Map(IEntityDescriptor<TEntity> descriptor);
}