using Nameless.Lucene.Repository.Mappings;
using Nameless.Registration;

namespace Nameless.Lucene;

/// <summary>
///     Registration options for the Lucene services, including analyzer selectors
///     and entity mappings.
/// </summary>
public class LuceneRegistration : AssemblyScanAware<LuceneRegistration> {
    private readonly HashSet<Type> _analyzerSelectors = [];
    private readonly HashSet<Type> _mappings = [];

    /// <summary>
    ///     Gets whether it should use Lucene repository feature.
    /// </summary>
    public bool UseRepository { get; private set; }

    /// <summary>
    ///     Gets the registered analyzer selector types.
    /// </summary>
    public IReadOnlyCollection<Type> AnalyzerSelectors => UseAssemblyScan
        ? ExecuteAssemblyScan(typeof(IAnalyzerSelector))
        : _analyzerSelectors;

    /// <summary>
    ///     Gets the registered entity mapping types.
    /// </summary>
    public IReadOnlyCollection<Type> Mappings => UseAssemblyScan
        ? ExecuteAssemblyScan(typeof(IEntityMapping<>))
        : _mappings;

    /// <summary>
    ///     Sets whether it should use Lucene repository feature.
    /// </summary>
    /// <param name="value"></param>
    /// <returns>
    ///     The current <see cref="LuceneRegistration"/> instance so other
    ///     actions can be chained.
    /// </returns>
    public LuceneRegistration WithUseRepository(bool value) {
        UseRepository = value;

        return this;
    }

    /// <summary>
    ///     Registers an analyzer selector by generic type parameter.
    /// </summary>
    /// <typeparam name="TAnalyzerSelector">The analyzer selector type.</typeparam>
    /// <returns>
    ///     The current <see cref="LuceneRegistration"/> instance so other
    ///     actions can be chained.
    /// </returns>
    public LuceneRegistration WithAnalyzerSelector<TAnalyzerSelector>()
        where TAnalyzerSelector : IAnalyzerSelector {
        return WithAnalyzerSelector(typeof(TAnalyzerSelector));
    }

    /// <summary>
    ///     Registers an analyzer selector by type.
    /// </summary>
    /// <param name="type">The analyzer selector type.</param>
    /// <returns>
    ///     The current <see cref="LuceneRegistration"/> instance so other
    ///     actions can be chained.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     if <paramref name="type"/> is not assignable from <see cref="IAnalyzerSelector"/>,
    ///     is an open generic type, or is a non-concrete type.
    /// </exception>
    public LuceneRegistration WithAnalyzerSelector(Type type) {
        Throws.When.IsNotAssignableFrom(type, typeof(IAnalyzerSelector));
        Throws.When.IsOpenGenericType(type);
        Throws.When.IsNonConcreteType(type);

        _analyzerSelectors.Add(type);

        return this;
    }

    /// <summary>
    ///     Registers an entity mapping by generic type parameters.
    /// </summary>
    /// <typeparam name="TEntityMapping">The entity mapping type.</typeparam>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <returns>
    ///     The current <see cref="LuceneRegistration"/> instance so other
    ///     actions can be chained.
    /// </returns>
    public LuceneRegistration WithMapping<TEntityMapping, TEntity>()
        where TEntityMapping : IEntityMapping<TEntity>
        where TEntity : class, new() {
        return WithMapping(typeof(TEntityMapping));
    }

    /// <summary>
    ///     Registers an entity mapping by type.
    /// </summary>
    /// <param name="type">The entity mapping type.</param>
    /// <returns>
    ///     The current <see cref="LuceneRegistration"/> instance so other
    ///     actions can be chained.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     if <paramref name="type"/> is not assignable from <see cref="IEntityMapping{TEntity}"/>,
    ///     is an open generic type, is a non-concrete type, or has no parameterless constructor.
    /// </exception>
    public LuceneRegistration WithMapping(Type type) {
        Throws.When.IsNotAssignableFromGeneric(type, typeof(IEntityMapping<>));
        Throws.When.IsOpenGenericType(type);
        Throws.When.IsNonConcreteType(type);
        Throws.When.HasNoParameterlessConstructor(type);

        _mappings.Add(type);

        return this;
    }
}
