namespace Nameless.Lucene.Repository.Mappings;

public interface IEntityDescriptorProvider {
    IEntityDescriptor<TEntity> GetDescriptor<TEntity>() where TEntity : class;
}
