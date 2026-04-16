using Nameless.Registration;

namespace Nameless.Lucene;

public class LuceneRegistrationSettings : AssemblyScanAware<LuceneRegistrationSettings> {
    private readonly HashSet<Type> _analyzerSelectors = [];

    public IReadOnlyCollection<Type> AnalyzerSelectors => _analyzerSelectors;

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
}
