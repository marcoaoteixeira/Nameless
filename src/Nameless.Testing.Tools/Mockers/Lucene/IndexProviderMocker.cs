using Moq;
using Nameless.Lucene;

namespace Nameless.Testing.Tools.Mockers.Lucene;

public class IndexProviderMocker : Mocker<IIndexProvider> {
    public IndexProviderMocker WithGet(IIndex returnValue) {
        MockInstance
            .Setup(mock => mock.Get(It.IsAny<string?>()))
            .Returns(returnValue);

        return this;
    }
}
