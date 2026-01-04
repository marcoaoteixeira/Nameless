using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Analysis.Util;
using Lucene.Net.Search;

namespace Nameless.Lucene;

/// <summary>
///     Contains all defaults for the library.
/// </summary>
public static class Defaults {
    /// <summary>
    ///     Gets the default maximum number of results that can be returned
    ///     by a query.
    /// </summary>
    public const int QUERY_MAXIMUM_RESULTS = short.MaxValue;

    /// <summary>
    ///     Gets the default <see cref="Sort"/>,
    /// </summary>
    public static Sort Sort => Sort.RELEVANCE;

    /// <summary>
    ///     Gets the default <see cref="Analyzer"/>,
    ///     <see cref="StandardAnalyzer"/>.
    /// </summary>
    public static Analyzer Analyzer => new StandardAnalyzer(
        matchVersion: Constants.CURRENT_VERSION,
        stopWords: new CharArraySet(
            Constants.CURRENT_VERSION,
            Enumerable.Empty<string>(),
            ignoreCase: true
        )
    );
}