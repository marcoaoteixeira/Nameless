using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Analysis.Util;
using Lucene.Net.Util;

namespace Nameless.Search.Lucene;

internal class Defaults {
    internal static readonly LuceneVersion Version = LuceneVersion.LUCENE_48;

    internal static CharArraySet EmptyCharArraySet = new(Version, Enumerable.Empty<string>(), true);

    internal static Analyzer Analyzer => new StandardAnalyzer(Version, EmptyCharArraySet);
}