using Lucene.Net.Analysis;
using Moq;
using Nameless.Lucene;

namespace Nameless.Testing.Tools.Mockers.Lucene;

public class AnalyzerProviderMocker : Mocker<IAnalyzerProvider> {
    public AnalyzerProviderMocker WithGetAnalyzer(Analyzer? returnValue = null) {
        MockInstance
            .Setup(mock => mock.GetAnalyzer(It.IsAny<string?>()))
            .Returns(returnValue ?? Nameless.Lucene.Defaults.Analyzer);

        return this;
    }
}
