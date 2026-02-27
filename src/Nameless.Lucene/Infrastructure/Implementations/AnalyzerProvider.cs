using Lucene.Net.Analysis;

namespace Nameless.Lucene.Infrastructure.Implementations;

/// <summary>
///     Default implementation of <see cref="IAnalyzerProvider" />.
/// </summary>
public class AnalyzerProvider : IAnalyzerProvider {
    private readonly IAnalyzerSelector[] _selectors;

    /// <summary>
    ///     Initializes a new instance of <see cref="AnalyzerProvider" />
    /// </summary>
    /// <param name="selectors">
    ///     The available selectors.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///     if <paramref name="selectors" /> is <see langword="null"/>
    /// </exception>
    public AnalyzerProvider(IEnumerable<IAnalyzerSelector> selectors) {
        _selectors = [.. selectors];
    }

    /// <inheritdoc />
    public Analyzer GetAnalyzer(string? indexName) {
        if (string.IsNullOrWhiteSpace(indexName)) {
            return Defaults.Analyzer;
        }

        var result = _selectors.Select(selector => selector.GetAnalyzer(indexName))
                               .MaxBy(result => result.Priority);

        return result?.Analyzer ?? Defaults.Analyzer;
    }
}