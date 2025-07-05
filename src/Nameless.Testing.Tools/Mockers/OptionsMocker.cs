using Microsoft.Extensions.Options;

namespace Nameless.Testing.Tools.Mockers;

public sealed class OptionsMocker<T> : Mocker<IOptions<T>> where T : class, new() {
    public OptionsMocker<T> WithValue(T returnValue) {
        MockInstance.Setup(mock => mock.Value)
                    .Returns(returnValue);

        return this;
    }
}