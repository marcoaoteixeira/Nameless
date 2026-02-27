using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Analysis.Util;
using Lucene.Net.Search;
using Lucene.Net.Util;

namespace Nameless.Lucene;

/// <summary>
///     Contains all defaults for the library.
/// </summary>
public static class Defaults {
    public static string IndexName => "ZGVmYXVsdC1sdWNlbmUtaW5kZXg=";
    
    /// <summary>
    ///     Gets the current version of Lucene being used.
    /// </summary>
    public static LuceneVersion CurrentVersion => LuceneVersion.LUCENE_48;

    /// <summary>
    ///     Gets the default <see cref="Sort"/>,
    /// </summary>
    public static Sort Sort => Sort.RELEVANCE;

    /// <summary>
    ///     Gets the default <see cref="Analyzer"/>,
    ///     <see cref="StandardAnalyzer"/>.
    /// </summary>
    public static Analyzer Analyzer => new StandardAnalyzer(
        matchVersion: CurrentVersion,
        stopWords: new CharArraySet(
            CurrentVersion,
            Enumerable.Empty<string>(),
            ignoreCase: true
        )
    );

    public static Type[] IndexableTypes { get; } = [
        // more common
        typeof(string),
        typeof(int),
        typeof(bool),
        typeof(DateTime),
        typeof(Enum),
        typeof(double),

        // less common
        typeof(sbyte),
        typeof(byte),
        typeof(ushort),
        typeof(short),
        typeof(uint),
        typeof(ulong),
        typeof(long),
        typeof(float),
        typeof(DateTimeOffset),
        typeof(DateOnly),
        typeof(TimeOnly),
        typeof(TimeSpan),
    ];
}