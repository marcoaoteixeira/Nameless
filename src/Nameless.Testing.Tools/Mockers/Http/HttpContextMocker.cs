using Microsoft.AspNetCore.Http;

namespace Nameless.Testing.Tools.Mockers.Http;

/// <summary>
///     Provides a mock implementation of the <see cref="HttpContext"/>
///     for use in unit testing scenarios.
/// </summary>
public class HttpContextMocker : Mocker<HttpContext>
{
    /// <summary>
    ///     Mocks the <see cref="HttpContext.Request"/> property.
    /// </summary>
    /// <param name="returnValue">
    ///     The return value.
    /// </param>
    /// <returns>
    ///     The current <see cref="HttpContextMocker"/> instance so other
    ///     actions can be chained.
    /// </returns>
    public HttpContextMocker WithRequest(HttpRequest returnValue)
    {
        MockInstance
            .Setup(mock => mock.Request)
            .Returns(returnValue);

        return this;
    }

    /// <summary>
    ///     Mocks the <see cref="HttpContext.Response"/> property.
    /// </summary>
    /// <param name="returnValue">
    ///     The return value.
    /// </param>
    /// <returns>
    ///     The current <see cref="HttpContextMocker"/> instance so other
    ///     actions can be chained.
    /// </returns>
    public HttpContextMocker WithResponse(HttpResponse returnValue)
    {
        MockInstance
            .Setup(mock => mock.Response)
            .Returns(returnValue);

        return this;
    }

    /// <summary>
    ///     Mocks the <see cref="HttpContext.Items"/> property.
    /// </summary>
    /// <param name="returnValue">
    ///     The return value.
    /// </param>
    /// <returns>
    ///     The current <see cref="HttpContextMocker"/> instance so other
    ///     actions can be chained.
    /// </returns>
    public HttpContextMocker WithItems(IDictionary<object, object?> returnValue)
    {
        MockInstance
            .Setup(mock => mock.Items)
            .Returns(returnValue);

        return this;
    }
}