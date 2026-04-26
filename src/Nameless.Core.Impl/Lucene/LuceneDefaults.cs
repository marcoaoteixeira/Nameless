using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Analysis.Util;
using Lucene.Net.Search;
using Lucene.Net.Util;

namespace Nameless.Lucene;

/// <summary>
///     Contains all defaults for the library.
/// </summary>
public static class LuceneDefaults {
    public static string IndexName => "ZGVmYXVsdC1sdWNlbmUtaW5kZXg=";
    
    /// <summary>
    ///     Gets the current version of Lucene being used.
    /// </summary>
    public static LuceneVersion Version => LuceneVersion.LUCENE_48;

    /// <summary>
    ///     Gets the default <see cref="Sort"/>,
    /// </summary>
    public static Sort Sort => Sort.RELEVANCE;

    /// <summary>
    ///     Gets the default <see cref="Analyzer"/>,
    ///     <see cref="StandardAnalyzer"/>.
    /// </summary>
    public static Analyzer Analyzer => new StandardAnalyzer(
        matchVersion: Version,
        stopWords: new CharArraySet(
            Version,
            Enumerable.Empty<string>(),
            ignoreCase: true
        )
    );
}