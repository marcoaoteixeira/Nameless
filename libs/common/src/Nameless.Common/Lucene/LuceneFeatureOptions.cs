using Nameless.Contracts;
using Nameless.Registration;

namespace Nameless.Lucene;

/// <summary>
///     Registration options for the Lucene services, including analyzer selectors
///     and entity mappings.
/// </summary>
public class LuceneFeatureOptions : AssemblyScanAware<LuceneFeatureOptions> {
    private readonly HashSet<Type> _analyzerSelectors = [];
    
    /// <summary>
    ///     Gets the registered analyzer selector types.
    /// </summary>
    public IReadOnlyCollection<Type> AnalyzerSelectors => UseAssemblyScan
        ? ExecuteAssemblyScan(typeof(IAnalyzerSelector))
        : _analyzerSelectors;

    /// <summary>
    ///     Registers an analyzer selector by generic type parameter.
    /// </summary>
    /// <typeparam name="TAnalyzerSelector">The analyzer selector type.</typeparam>
    /// <returns>
    ///     The current <see cref="LuceneFeatureOptions"/> instance so other
    ///     actions can be chained.
    /// </returns>
    public LuceneFeatureOptions WithAnalyzerSelector<TAnalyzerSelector>()
        where TAnalyzerSelector : IAnalyzerSelector {
        return WithAnalyzerSelector(typeof(TAnalyzerSelector));
    }

    /// <summary>
    ///     Registers an analyzer selector by type.
    /// </summary>
    /// <param name="type">The analyzer selector type.</param>
    /// <returns>
    ///     The current <see cref="LuceneFeatureOptions"/> instance so other
    ///     actions can be chained.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     if <paramref name="type"/> is not assignable from <see cref="IAnalyzerSelector"/>,
    ///     is an open generic type, or is a non-concrete type.
    /// </exception>
    public LuceneFeatureOptions WithAnalyzerSelector(Type type) {
        Throws.When.OpenGenericType(type);
        Throws.When.NonConcreteType(type);
        Throws.When.NonAssignableFrom(type, typeof(IAnalyzerSelector));

        _analyzerSelectors.Add(type);

        return this;
    }
}
