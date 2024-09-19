using Lucene.Net.Analysis;

namespace Nameless.Lucene;

/// <summary>
/// Represents a Lucene analyzer selector result.
/// </summary>
public sealed record AnalyzerSelectorResult {
    /// <summary>
    /// Gets or sets the priority.
    /// </summary>
    public int Priority { get; }

    /// <summary>
    /// Gets or sets the analyzer instance.
    /// </summary>
    public Analyzer Analyzer { get; }

    public AnalyzerSelectorResult(Analyzer analyzer, int priority = 0) {
        Analyzer = Prevent.Argument.Null(analyzer);
        Priority = priority;
    }
}