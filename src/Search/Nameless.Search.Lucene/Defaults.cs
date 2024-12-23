using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Util;

namespace Nameless.Search.Lucene;

internal class Defaults {
    internal static readonly LuceneVersion Version = LuceneVersion.LUCENE_48;
    internal static Analyzer Analyzer => new StandardAnalyzer(matchVersion: Version);
}
