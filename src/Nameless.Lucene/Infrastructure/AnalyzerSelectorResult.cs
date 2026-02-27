using Lucene.Net.Analysis;

namespace Nameless.Lucene.Infrastructure;

/// <summary>
///     Represents an analyzer selector result.
/// </summary>
public record AnalyzerSelectorResult(Analyzer? Analyzer, int Priority = 0);