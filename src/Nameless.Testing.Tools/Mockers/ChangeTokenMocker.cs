using Microsoft.Extensions.Primitives;
using Moq;

namespace Nameless.Testing.Tools.Mockers;

public sealed class ChangeTokenMocker : Mocker<IChangeToken> {
    public ChangeTokenMocker WithHasChanged(bool returnValue) {
        MockInstance.Setup(mock => mock.HasChanged)
                    .Returns(returnValue);

        return this;
    }

    public ChangeTokenMocker WithActiveChangeCallbacks(bool returnValue) {
        MockInstance.Setup(mock => mock.ActiveChangeCallbacks)
                    .Returns(returnValue);

        return this;
    }

    public ChangeTokenMocker WithRegisterChangeCallback(IDisposable returnValue) {
        MockInstance.Setup(mock => mock.RegisterChangeCallback(It.IsAny<Action<object?>>(), It.IsAny<object>()))
                    .Returns(returnValue);

        return this;
    }
}