using Lucene.Net.Analysis;
using Moq;
using Nameless.Mockers;

namespace Nameless.Lucene.Mockers;
public class AnalyzerSelectorMocker : MockerBase<IAnalyzerSelector> {
    public AnalyzerSelectorMocker WithAnalyzerFor(string indexName, Analyzer analyzer = null, int priority = 0) {
        var innerAnalyzer = analyzer ?? Mock.Of<Analyzer>();

        InnerMock.Setup(mock => mock.GetAnalyzer(indexName))
                 .Returns(new AnalyzerSelectorResult(innerAnalyzer, priority));

        return this;
    }
}
