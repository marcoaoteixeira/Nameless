using Nameless.Lucene.Repository.Mappings;
using Nameless.Registration;

namespace Nameless.Lucene.Repository;

public class LuceneRepositoryRegistrationSettings : AssemblyScanAware<LuceneRepositoryRegistrationSettings> {
    private readonly HashSet<Type> _mappings = [];

    public IReadOnlyCollection<Type> Mappings => _mappings;

    public LuceneRepositoryRegistrationSettings RegisterMappings<TEntityMapping, TEntity>()
        where TEntityMapping : IEntityMapping<TEntity>
        where TEntity : class, new() {
        return RegisterMapping(typeof(TEntityMapping));
    }

    public LuceneRepositoryRegistrationSettings RegisterMapping(Type type) {
        Throws.When.IsNotAssignableFromGeneric(type, typeof(IEntityMapping<>));
        Throws.When.IsOpenGenericType(type);
        Throws.When.IsNonConcreteType(type);
        Throws.When.HasNoParameterlessConstructor(type);

        _mappings.Add(type);

        return this;
    }
}