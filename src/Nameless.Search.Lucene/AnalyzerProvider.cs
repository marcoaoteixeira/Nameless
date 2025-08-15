using Lucene.Net.Analysis;
using Microsoft.Extensions.Options;

namespace Nameless.Search.Lucene;

/// <summary>
///     Default implementation of <see cref="IAnalyzerProvider" />.
/// </summary>
public sealed class AnalyzerProvider : IAnalyzerProvider {
    private readonly IOptions<SearchOptions> _options;

    /// <summary>
    ///     Initializes a new instance of <see cref="AnalyzerProvider" />
    /// </summary>
    /// <param name="options">
    ///     The options.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///     if <paramref name="options" /> is <see langword="null"/>
    /// </exception>
    public AnalyzerProvider(IOptions<SearchOptions> options) {
        _options = Guard.Against.Null(options);
    }

    /// <inheritdoc />
    public Analyzer GetAnalyzer(string indexName) {
        var result = _options.Value.AnalyzerSelectors
                             .Select(selector => selector.GetAnalyzer(indexName))
                             .MaxBy(item => item.Priority);

        return result?.Analyzer ?? Defaults.Analyzer;
    }
}
