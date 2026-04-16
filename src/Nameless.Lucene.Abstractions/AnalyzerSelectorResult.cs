using Lucene.Net.Analysis;

namespace Nameless.Lucene;

/// <summary>
///     Represents an analyzer selector result.
/// </summary>
public record AnalyzerSelectorResult(Analyzer? Analyzer, int Priority = 0);