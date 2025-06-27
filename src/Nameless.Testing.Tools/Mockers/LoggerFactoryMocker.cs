using Microsoft.Extensions.Logging;
using Moq;

namespace Nameless.Testing.Tools.Mockers;

public sealed class LoggerFactoryMocker : Mocker<ILoggerFactory> {
    public LoggerFactoryMocker WithCreateLogger(ILogger returnValue) {
        MockInstance.Setup(mock => mock.CreateLogger(It.IsAny<string>()))
                    .Returns(returnValue);

        return this;
    }

    public LoggerFactoryMocker WithCreateLogger(string category, ILogger returnValue) {
        MockInstance.Setup(mock => mock.CreateLogger(category))
                    .Returns(returnValue);

        return this;
    }

    public LoggerFactoryMocker WithCreateLogger<TCategoryName>(ILogger<TCategoryName> returnValue) {
        MockInstance.Setup(mock => mock.CreateLogger(typeof(TCategoryName).FullName ?? typeof(TCategoryName).Name))
                    .Returns(returnValue);

        return this;
    }
}