using System.Linq.Expressions;
using Moq;
using Nameless.Testing.Tools.Mockers;

namespace Nameless.Mediator.Fixtures;

public interface IPrintService {
    void Print(string message);
}

public class PrintServiceMocker : Mocker<IPrintService> {
    public PrintServiceMocker WithPrintCallback(Action<string> callback) {
        MockInstance
            .Setup(mock => mock.Print(It.IsAny<string>()))
            .Callback(callback);

        return this;
    }

    public void VerifyPrintCall(int times = 1) {
        MockInstance.Verify(mock => mock.Print(It.IsAny<string>()), Times.Exactly(times));
    }

    public void VerifyPrintCall(Expression<Func<string, bool>> assertMessage, int times = 1) {
        MockInstance.Verify(mock => mock.Print(It.Is(assertMessage)), Times.Exactly(times));
    }
}