using Lucene.Net.Analysis;

namespace Nameless.Search.Lucene;

/// <summary>
/// Default implementation of <see cref="IAnalyzerProvider"/>.
/// </summary>
public sealed class AnalyzerProvider : IAnalyzerProvider {
    private readonly IAnalyzerSelector[] _selectors;

    /// <summary>
    /// Initializes a new instance of <see cref="AnalyzerProvider"/>
    /// </summary>
    /// <param name="selectors">A collection of <see cref="IAnalyzerSelector"/></param>
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="selectors"/> is <c>null</c>
    /// </exception>
    public AnalyzerProvider(IEnumerable<IAnalyzerSelector> selectors) {
        _selectors = Prevent.Argument.Null(selectors).ToArray();
    }

    /// <inheritdoc />
    public Analyzer GetAnalyzer(string indexName) {
        var selectors = _selectors.Select(selector => selector.GetAnalyzer(indexName));

        var selector = selectors
#if NET8_0_OR_GREATER
            .MaxBy(selector => selector.Priority);
#else
            .OrderByDescending(selector => selector.Priority)
            .FirstOrDefault();
#endif

        return selector?.Analyzer ?? Defaults.Analyzer;
    }
}