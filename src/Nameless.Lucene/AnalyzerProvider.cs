using Lucene.Net.Analysis;
using Microsoft.Extensions.Options;

namespace Nameless.Lucene;

/// <summary>
///     Default implementation of <see cref="IAnalyzerProvider" />.
/// </summary>
public sealed class AnalyzerProvider : IAnalyzerProvider {
    private readonly IOptions<LuceneOptions> _options;

    /// <summary>
    ///     Initializes a new instance of <see cref="AnalyzerProvider" />
    /// </summary>
    /// <param name="options">
    ///     The options.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///     if <paramref name="options" /> is <see langword="null"/>
    /// </exception>
    public AnalyzerProvider(IOptions<LuceneOptions> options) {
        _options = options;
    }

    /// <inheritdoc />
    public Analyzer GetAnalyzer(string indexName) {
        var result = _options.Value
                             .AnalyzerSelectors
                             .Select(selector => selector.GetAnalyzer(indexName))
                             .MaxBy(result => result.Priority);

        return result?.Analyzer ?? Defaults.Analyzer;
    }
}