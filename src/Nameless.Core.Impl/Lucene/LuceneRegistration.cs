using Nameless.Lucene.Repository.Mappings;
using Nameless.Registration;

namespace Nameless.Lucene;

public class LuceneRegistration : AssemblyScanAware<LuceneRegistration> {
    private readonly HashSet<Type> _analyzerSelectors = [];
    private readonly HashSet<Type> _mappings = [];

    public bool UseRepository { get; set; }

    public IReadOnlyCollection<Type> AnalyzerSelectors => _analyzerSelectors;

    public IReadOnlyCollection<Type> Mappings => _mappings;

    public LuceneRegistration RegisterAnalyzerSelector<TAnalyzerSelector>()
        where TAnalyzerSelector : IAnalyzerSelector {
        return RegisterAnalyzerSelector(typeof(TAnalyzerSelector));
    }

    public LuceneRegistration RegisterAnalyzerSelector(Type type) {
        Throws.When.IsNotAssignableFrom(type, typeof(IAnalyzerSelector));
        Throws.When.IsOpenGenericType(type);
        Throws.When.IsNonConcreteType(type);

        _analyzerSelectors.Add(type);

        return this;
    }

    public LuceneRegistration RegisterMappings<TEntityMapping, TEntity>()
        where TEntityMapping : IEntityMapping<TEntity>
        where TEntity : class, new() {
        return RegisterMapping(typeof(TEntityMapping));
    }

    public LuceneRegistration RegisterMapping(Type type) {
        Throws.When.IsNotAssignableFromGeneric(type, typeof(IEntityMapping<>));
        Throws.When.IsOpenGenericType(type);
        Throws.When.IsNonConcreteType(type);
        Throws.When.HasNoParameterlessConstructor(type);

        _mappings.Add(type);

        return this;
    }
}
