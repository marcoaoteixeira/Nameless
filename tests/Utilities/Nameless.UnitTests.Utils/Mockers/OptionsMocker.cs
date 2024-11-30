using Microsoft.Extensions.Options;

namespace Nameless.Mockers;

public class OptionsMocker<T> : MockerBase<IOptions<T>> where T : class, new() {
    public OptionsMocker() {
        InnerMock.Setup(mock => mock.Value)
                 .Returns(new T());
    }
}