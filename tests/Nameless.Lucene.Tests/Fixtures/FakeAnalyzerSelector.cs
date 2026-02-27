using Nameless.Lucene.Infrastructure;

namespace Nameless.Lucene.Fixtures;

public sealed class FakeAnalyzerSelector : IAnalyzerSelector {
    private readonly string _indexName;

    public FakeAnalyzerSelector(string indexName) {
        _indexName = indexName;
    }

    public AnalyzerSelectorResult GetAnalyzer(string indexName) {
        return new AnalyzerSelectorResult(
            Analyzer: string.Equals(indexName, _indexName)
                ? new FakeAnalyzer()
                : null,
            Priority: 0
        );
    }
}
