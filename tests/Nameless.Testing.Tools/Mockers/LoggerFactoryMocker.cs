using Microsoft.Extensions.Logging;

namespace Nameless.Testing.Tools.Mockers;

public sealed class LoggerFactoryMocker : MockerBase<ILoggerFactory> {
    public LoggerFactoryMocker WithCreateLogger(string category, ILogger logger) {
        MockInstance.Setup(mock => mock.CreateLogger(category))
                    .Returns(logger);

        return this;
    }

    public LoggerFactoryMocker WithCreateLogger<TCategory>(ILogger<TCategory> logger) {
        MockInstance.Setup(mock => mock.CreateLogger(typeof(TCategory).FullName))
                    .Returns(logger);

        return this;
    }
}