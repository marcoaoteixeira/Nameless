using Moq;
using Nameless.Bootstrap;

namespace Nameless.Testing.Tools.Mockers.Bootstrap;

public class BootstrapperMocker : Mocker<IBootstrapper> {
    public BootstrapperMocker WithExecuteAsync(Func<Task>? returnValue = null) {
        MockInstance.Setup(mock => mock.ExecuteAsync(It.IsAny<CancellationToken>()))
                    .Returns(returnValue ?? (() => Task.CompletedTask));

        return this;
    }

    public BootstrapperMocker ThrowsOnExecuteAsync<TException>()
        where TException : Exception, new() {
        MockInstance.Setup(mock => mock.ExecuteAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(new TException());

        return this;
    }
    
    public BootstrapperMocker ThrowsOnExecuteAsync<TException>(TException exception)
        where TException : Exception {
        MockInstance.Setup(mock => mock.ExecuteAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(exception);

        return this;
    }

    public void VerifyExecuteAsync(int times = 1) {
        Verify(mock => mock.ExecuteAsync(It.IsAny<CancellationToken>()), times, exactly: true);
    }
}
