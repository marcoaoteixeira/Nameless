using Nameless.Infrastructure;

namespace Nameless.Lucene;

/// <summary>
///     Lucene Options
/// </summary>
public class LuceneOptions {
    private readonly HashSet<IAnalyzerSelector> _analyzerSelectors = [];

    /// <summary>
    ///     Gets the types of analyzers that will be used for indexing and searching.
    /// </summary>
    internal IReadOnlyCollection<IAnalyzerSelector> AnalyzerSelectors => _analyzerSelectors;

    internal string DirectoryName {
        get => field ?? "Indexes";
        set;
    }

    /// <summary>
    ///     Registers an analyzer selector.
    /// </summary>
    /// <typeparam name="TAnalyzerSelector">
    ///     Type of the analyzer selector.
    /// </typeparam>
    /// <param name="analyzerSelector">
    ///     The analyzer selector to register.
    /// </param>
    /// <returns>
    ///     The current <see cref="LuceneOptions"/> so other actions can be chained.
    /// </returns>
    public LuceneOptions RegisterAnalyzerSelector<TAnalyzerSelector>(TAnalyzerSelector analyzerSelector)
        where TAnalyzerSelector : class, IAnalyzerSelector {
        _analyzerSelectors.Add(analyzerSelector);

        return this;
    }

    /// <summary>
    ///     Sets the root directory name that will store
    ///     all Lucene indexes. This directory must be relative to the
    ///     <see cref="IApplicationContext.DataDirectoryPath"/>.
    /// </summary>
    /// <param name="directoryName">
    ///     The directory name.
    /// </param>
    /// <returns>
    ///     The current <see cref="LuceneOptions"/> so other actions
    ///     can be chained.
    /// </returns>
    /// <remarks>
    ///     The default value is <c>indexes</c>.
    /// </remarks>
    public LuceneOptions SetDirectoryName(string directoryName) {
        DirectoryName = Guard.Against.NullOrWhiteSpace(directoryName);

        return this;
    }
}