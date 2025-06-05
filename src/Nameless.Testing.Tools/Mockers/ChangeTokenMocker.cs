using Microsoft.Extensions.Primitives;
using Moq;

namespace Nameless.Testing.Tools.Mockers;

public sealed class ChangeTokenMocker : MockerBase<IChangeToken> {
    public ChangeTokenMocker WithHasChanged(bool hasChanged) {
        MockInstance.Setup(mock => mock.HasChanged)
                    .Returns(hasChanged);

        return this;
    }

    public ChangeTokenMocker WithActiveChangeCallbacks(bool activeChangeCallbacks) {
        MockInstance.Setup(mock => mock.ActiveChangeCallbacks)
                    .Returns(activeChangeCallbacks);

        return this;
    }

    public ChangeTokenMocker WithRegisterChangeCallback(IDisposable changeCallbackRegistration) {
        MockInstance.Setup(mock => mock.RegisterChangeCallback(It.IsAny<Action<object?>>(), It.IsAny<object>()))
                    .Returns(changeCallbackRegistration);

        return this;
    }
}