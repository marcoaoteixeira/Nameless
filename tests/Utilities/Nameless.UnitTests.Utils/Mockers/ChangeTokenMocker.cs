using Microsoft.Extensions.Primitives;
using Moq;

namespace Nameless.Mockers;

public class ChangeTokenMocker : MockerBase<IChangeToken> {
    public ChangeTokenMocker WithHasChanged(bool hasChanged) {
        Mock.Setup(mock => mock.HasChanged)
                 .Returns(hasChanged);

        return this;
    }

    public ChangeTokenMocker WithActiveChangeCallbacks(bool activeChangeCallbacks) {
        Mock.Setup(mock => mock.ActiveChangeCallbacks)
                 .Returns(activeChangeCallbacks);

        return this;
    }

    public ChangeTokenMocker WithRegisterChangeCallback(IDisposable changeCallbackRegistration) {
        Mock.Setup(mock => mock.RegisterChangeCallback(It.IsAny<Action<object>>(), It.IsAny<object>()))
                 .Returns(changeCallbackRegistration);

        return this;
    }
}
