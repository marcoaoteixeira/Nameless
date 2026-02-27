using Lucene.Net.Analysis;
using Moq;
using Nameless.Lucene.Infrastructure;
using Nameless.Testing.Tools.Mockers;

namespace Nameless.Lucene.Mockers;

public class AnalyzerProviderMocker : Mocker<IAnalyzerProvider> {
    public AnalyzerProviderMocker WithGetAnalyzer(Analyzer returnValue) {
        MockInstance.Setup(mock => mock.GetAnalyzer(It.IsAny<string>()))
                    .Returns(returnValue);

        return this;
    }

    public void VerifyGetAnalyzer(int times, bool exactly = false) {
        Verify(mock => mock.GetAnalyzer(It.IsAny<string>()), times, exactly);
    }
}
