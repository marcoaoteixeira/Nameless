using Microsoft.Extensions.Logging;
using Moq;

namespace Nameless.Testing.Tools.Mockers.Logging;

/// <summary>
///     <see cref="ILoggerFactory"/> mocker implementation.
/// </summary>
public class LoggerFactoryMocker : Mocker<ILoggerFactory> {
    /// <summary>
    ///     Defines which object to return when calling
    ///     <see cref="ILoggerFactory.CreateLogger"/> method.
    /// </summary>
    /// <param name="returnValue">
    ///     The returned value.
    /// </param>
    /// <returns>
    ///     The current <see cref="LoggerFactoryMocker"/> instance
    ///     so other actions can be chained.
    /// </returns>
    public LoggerFactoryMocker WithCreateLogger(ILogger returnValue) {
        MockInstance.Setup(mock => mock.CreateLogger(It.IsAny<string>()))
                    .Returns(returnValue);

        return this;
    }

    /// <summary>
    ///     Defines which object to return when calling
    ///     <see cref="ILoggerFactory.CreateLogger"/> method with
    ///     the specified parameters.
    /// </summary>
    /// <param name="categoryName">
    ///     The category name.
    /// </param>
    /// <param name="returnValue">
    ///     The returned value.
    /// </param>
    /// <returns>
    ///     The current <see cref="LoggerFactoryMocker"/> instance
    ///     so other actions can be chained.
    /// </returns>
    public LoggerFactoryMocker WithCreateLogger(string categoryName, ILogger returnValue) {
        MockInstance.Setup(mock => mock.CreateLogger(categoryName))
                    .Returns(returnValue);

        return this;
    }

    /// <summary>
    ///     Defines which object to return when calling
    ///     <see cref="ILoggerFactory.CreateLogger"/> method with
    ///     the specified parameters.
    /// </summary>
    /// <param name="returnValue">
    ///     The returned value.
    /// </param>
    /// <returns>
    ///     The current <see cref="LoggerFactoryMocker"/> instance
    ///     so other actions can be chained.
    /// </returns>
    public LoggerFactoryMocker WithCreateLogger<TCategoryName>(ILogger<TCategoryName> returnValue) {
        MockInstance.Setup(mock => mock.CreateLogger(typeof(TCategoryName).FullName ?? typeof(TCategoryName).Name))
                    .Returns(returnValue);

        return this;
    }

    /// <summary>
    ///     Defines which object to return when calling
    ///     <see cref="ILoggerFactory.CreateLogger"/> method with
    ///     the specified parameters.
    /// </summary>
    /// <param name="category">
    ///     The category type.
    /// </param>
    /// <param name="returnValue">
    ///     The returned value.
    /// </param>
    /// <returns>
    ///     The current <see cref="LoggerFactoryMocker"/> instance
    ///     so other actions can be chained.
    /// </returns>
    public LoggerFactoryMocker WithCreateLogger(Type category, ILogger returnValue) {
        MockInstance.Setup(mock => mock.CreateLogger(category.FullName ?? category.Name))
                    .Returns(returnValue);

        return this;
    }
}