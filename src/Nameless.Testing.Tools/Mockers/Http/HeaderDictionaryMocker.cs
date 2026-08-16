using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Moq;

namespace Nameless.Testing.Tools.Mockers.Http;

/// <summary>
///     Provides a mock implementation of the <see cref="IHeaderDictionary"/>
///     for use in unit testing scenarios.
/// </summary>
public class HeaderDictionaryMocker : Mocker<IHeaderDictionary>
{
    /// <summary>
    ///     Mocks the <see cref="HttpRequest.Headers"/> property.
    /// </summary>
    /// <param name="returnValue">
    ///     The return value.
    /// </param>
    /// <param name="output">
    ///     The output value.
    /// </param>
    /// <returns>
    ///     The current <see cref="HeaderDictionaryMocker"/> instance so other
    ///     actions can be chained.
    /// </returns>
    public HeaderDictionaryMocker WithTryGetValue(bool returnValue, StringValues output)
    {
        MockInstance
            .Setup(mock => mock.TryGetValue(It.IsAny<string>(), out output))
            .Returns(returnValue);

        return this;
    }
}