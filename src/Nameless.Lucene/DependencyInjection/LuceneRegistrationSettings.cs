using Nameless.Lucene.Infrastructure;
using Nameless.Lucene.Mapping;
using Nameless.Registration;

namespace Nameless.Lucene;

public class LuceneRegistrationSettings : AssemblyScanAware<LuceneRegistrationSettings> {
    private readonly HashSet<Type> _analyzerSelectors = [];
    private readonly HashSet<Type> _mappings = [];

    public IReadOnlyCollection<Type> AnalyzerSelectors => UseAssemblyScan
        ? DiscoverImplementationsFor<IAnalyzerSelector>()
        : _analyzerSelectors;

    public IReadOnlyCollection<Type> Mappings => UseAssemblyScan
        ? DiscoverImplementationsFor(typeof(EntityMapping<>))
        : _mappings;

    public LuceneRegistrationSettings RegisterAnalyzerSelector<TAnalyzerSelector>()
        where TAnalyzerSelector : IAnalyzerSelector {
        return RegisterAnalyzerSelector(typeof(TAnalyzerSelector));
    }

    public LuceneRegistrationSettings RegisterAnalyzerSelector(Type type) {
        Throws.When.IsNotAssignableFrom(type, typeof(IAnalyzerSelector));
        Throws.When.IsOpenGenericType(type);
        Throws.When.IsNonConcreteType(type);

        _analyzerSelectors.Add(type);

        return this;
    }

    public LuceneRegistrationSettings RegisterMappings<TEntityMapping, TEntity>()
        where TEntityMapping : EntityMapping<TEntity>
        where TEntity : class, new() {
        return RegisterMapping(typeof(TEntityMapping));
    }

    public LuceneRegistrationSettings RegisterMapping(Type type) {
        Throws.When.IsNotAssignableFromGeneric(type, typeof(EntityMapping<>));
        Throws.When.IsOpenGenericType(type);
        Throws.When.IsNonConcreteType(type);
        Throws.When.HasNoParameterlessConstructor(type);

        _mappings.Add(type);

        return this;
    }
}
