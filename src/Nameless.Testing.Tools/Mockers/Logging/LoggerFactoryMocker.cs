using Microsoft.Extensions.Logging;
using Moq;

namespace Nameless.Testing.Tools.Mockers.Logging;

public class LoggerFactoryMocker : Mocker<ILoggerFactory> {
    public LoggerFactoryMocker WithCreateLogger(ILogger returnValue) {
        MockInstance.Setup(mock => mock.CreateLogger(It.IsAny<string>()))
                    .Returns(returnValue);

        return this;
    }

    public LoggerFactoryMocker WithCreateLogger(string categoryName, ILogger returnValue) {
        MockInstance.Setup(mock => mock.CreateLogger(categoryName))
                    .Returns(returnValue);

        return this;
    }

    public LoggerFactoryMocker WithCreateLogger<TCategoryName>(ILogger<TCategoryName> returnValue) {
        MockInstance.Setup(mock => mock.CreateLogger(typeof(TCategoryName).FullName ?? typeof(TCategoryName).Name))
                    .Returns(returnValue);

        return this;
    }

    public LoggerFactoryMocker WithCreateLogger(Type category, ILogger returnValue) {
        MockInstance.Setup(mock => mock.CreateLogger(category.FullName ?? category.Name))
                    .Returns(returnValue);

        return this;
    }
}