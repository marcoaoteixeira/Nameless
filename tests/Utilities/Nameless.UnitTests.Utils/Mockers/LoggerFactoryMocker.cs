using Microsoft.Extensions.Logging;

namespace Nameless.Mockers;
public sealed class LoggerFactoryMocker : MockerBase<ILoggerFactory> {
    public LoggerFactoryMocker WithCreateLogger<TCategory>(ILogger<TCategory> logger) {
        Mock.Setup(mock => mock.CreateLogger(typeof(TCategory).FullName))
                 .Returns(logger);

        return this;
    }
}
