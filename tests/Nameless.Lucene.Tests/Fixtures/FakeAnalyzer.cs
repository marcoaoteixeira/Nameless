using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Util;

namespace Nameless.Lucene.Fixtures;

public sealed class FakeAnalyzer : Analyzer {
    protected override TokenStreamComponents CreateComponents(string fieldName, TextReader reader) {
        return new TokenStreamComponents(
            new ClassicTokenizer(
                LuceneVersion.LUCENE_48,
                TextReader.Null
            )
        );
    }
}
