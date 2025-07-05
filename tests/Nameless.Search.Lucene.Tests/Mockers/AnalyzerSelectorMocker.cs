using Lucene.Net.Analysis;
using Moq;
using Nameless.Testing.Tools.Mockers;

namespace Nameless.Search.Lucene.Mockers;

public class AnalyzerSelectorMocker : Mocker<IAnalyzerSelector> {
    public AnalyzerSelectorMocker WithAnalyzerFor(string indexName, Analyzer analyzer = null, int priority = 0) {
        var innerAnalyzer = analyzer ?? Mock.Of<Analyzer>();

        MockInstance.Setup(mock => mock.GetAnalyzer(indexName))
                    .Returns(new AnalyzerSelectorResult(innerAnalyzer, priority));

        return this;
    }
}