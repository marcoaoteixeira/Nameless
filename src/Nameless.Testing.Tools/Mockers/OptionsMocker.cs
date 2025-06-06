using Microsoft.Extensions.Options;

namespace Nameless.Testing.Tools.Mockers;

public sealed class OptionsMocker<T> : MockerBase<IOptions<T>> where T : class, new() {
    public OptionsMocker<T> WithValue(T value) {
        MockInstance.Setup(mock => mock.Value)
                    .Returns(value);

        return this;
    }
}