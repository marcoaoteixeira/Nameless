using Lucene.Net.Analysis;

namespace Nameless.Search.Lucene;

/// <summary>
///     Represents an analyzer selector result.
/// </summary>
public sealed record AnalyzerSelectorResult {
    /// <summary>
    ///     Gets the priority.
    /// </summary>
    public int Priority { get; }

    /// <summary>
    ///     Gets the analyzer.
    /// </summary>
    public Analyzer Analyzer { get; }

    /// <summary>
    ///     Initializes a new instance of <see cref="AnalyzerSelectorResult" />
    /// </summary>
    /// <param name="analyzer">The analyzer</param>
    /// <param name="priority">The priority</param>
    /// <exception cref="ArgumentNullException">
    ///     if <paramref name="analyzer" /> is <see langword="null"/>
    /// </exception>
    public AnalyzerSelectorResult(Analyzer analyzer, int priority = 0) {
        Analyzer = Guard.Against.Null(analyzer);
        Priority = priority;
    }
}