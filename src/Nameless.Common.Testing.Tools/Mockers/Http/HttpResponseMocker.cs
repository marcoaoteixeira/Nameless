using Microsoft.AspNetCore.Http;

namespace Nameless.Testing.Tools.Mockers.Http;

/// <summary>
///     Provides a mock implementation of the <see cref="HttpResponse"/>
///     for use in unit testing scenarios.
/// </summary>
public class HttpResponseMocker : Mocker<HttpResponse>
{
    /// <summary>
    ///     Mocks the <see cref="HttpResponse.Headers"/> property.
    /// </summary>
    /// <param name="returnValue">
    ///     The return value.
    /// </param>
    /// <returns>
    ///     The current <see cref="HttpResponseMocker"/> instance so other
    ///     actions can be chained.
    /// </returns>
    public HttpResponseMocker WithHeader(IHeaderDictionary returnValue)
    {
        MockInstance
            .Setup(mock => mock.Headers)
            .Returns(returnValue);

        return this;
    }
}