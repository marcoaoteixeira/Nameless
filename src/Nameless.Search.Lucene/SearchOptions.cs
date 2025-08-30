namespace Nameless.Search.Lucene;

/// <summary>
///     Lucene Options
/// </summary>
public sealed class SearchOptions {
    private readonly HashSet<IAnalyzerSelector> _analyzerSelectors = [];

    /// <summary>
    ///     Gets or sets the folder name that will store Lucene.NET indexes.
    /// </summary>
    public string IndexesDirectoryName { get; set; } = "Indexes";

    /// <summary>
    ///     Gets the types of analyzers that will be used for indexing and searching.
    /// </summary>
    public IEnumerable<IAnalyzerSelector> AnalyzerSelectors => _analyzerSelectors;

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
    ///     The current <see cref="SearchOptions"/> so other actions can be chained.
    /// </returns>
    public SearchOptions RegisterAnalyzerSelector<TAnalyzerSelector>(TAnalyzerSelector analyzerSelector)
        where TAnalyzerSelector : class, IAnalyzerSelector {
        _analyzerSelectors.Add(analyzerSelector);

        return this;
    }
}