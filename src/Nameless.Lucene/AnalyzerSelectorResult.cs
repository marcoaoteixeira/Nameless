using Lucene.Net.Analysis;

namespace Nameless.Lucene;

/// <summary>
///     Represents an analyzer selector result.
/// </summary>
public sealed record AnalyzerSelectorResult {
    /// <summary>
    ///     Gets the priority. Higher values indicate higher priority.
    /// </summary>
    public int Priority { get; }

    /// <summary>
    ///     Gets the analyzer.
    /// </summary>
    public Analyzer? Analyzer { get; }

    /// <summary>
    ///     Initializes a new instance of <see cref="AnalyzerSelectorResult" />
    ///     class.
    /// </summary>
    /// <param name="analyzer">
    ///     The analyzer.
    /// </param>
    /// <param name="priority">
    ///     The priority.
    /// </param>
    public AnalyzerSelectorResult(Analyzer? analyzer, int priority = 0) {
        Analyzer = analyzer;
        Priority = priority;
    }
}