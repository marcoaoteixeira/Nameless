using Microsoft.AspNetCore.Http;

namespace Nameless.Testing.Tools.Mockers.Http;

/// <summary>
///     Provides a mock implementation of the <see cref="HttpRequest"/>
///     for use in unit testing scenarios.
/// </summary>
public class HttpRequestMocker : Mocker<HttpRequest>
{
    /// <summary>
    ///     Mocks the <see cref="HttpRequest.Headers"/> property.
    /// </summary>
    /// <param name="returnValue">
    ///     The return value.
    /// </param>
    /// <returns>
    ///     The current <see cref="HttpRequestMocker"/> instance so other
    ///     actions can be chained.
    /// </returns>
    public HttpRequestMocker WithHeader(IHeaderDictionary returnValue)
    {
        MockInstance
            .Setup(mock => mock.Headers)
            .Returns(returnValue);

        return this;
    }
}