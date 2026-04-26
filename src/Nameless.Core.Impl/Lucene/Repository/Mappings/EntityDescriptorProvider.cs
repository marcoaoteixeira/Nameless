using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;

namespace Nameless.Lucene.Repository.Mappings;

public class EntityDescriptorProvider : IEntityDescriptorProvider {
    private static readonly ConcurrentDictionary<Type, object> Cache = [];

    private readonly IServiceProvider _provider;

    public EntityDescriptorProvider(IServiceProvider provider) {
        _provider = provider;
    }

    public IEntityDescriptor<TEntity> GetDescriptor<TEntity>() where TEntity : class {
        var descriptor = Cache.GetOrAdd(
            key: typeof(TEntity),
            valueFactory: _ => InnerGetDescriptor<TEntity>()
        );

        return (EntityDescriptor<TEntity>)descriptor;
    }

    private EntityDescriptor<TEntity> InnerGetDescriptor<TEntity>() where TEntity : class {
        var mapping = _provider.GetRequiredService<IEntityMapping<TEntity>>();
        var descriptor = new EntityDescriptor<TEntity>();

        mapping.Map(descriptor);

        return descriptor.HasID
            ? descriptor
            : throw new MissingEntityIDException(typeof(TEntity));
    }
}
