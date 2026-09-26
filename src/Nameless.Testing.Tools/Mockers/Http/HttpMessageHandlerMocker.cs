using System.Net;
using System.Text;
using System.Text.Json;
using Moq;
using Moq.Protected;

namespace Nameless.Testing.Tools.Mockers.Http;

/// <summary>
///     Provides a mock implementation of <see cref="HttpMessageHandler"/>
///     for use in unit testing HTTP client code.
/// </summary>
public class HttpMessageHandlerMocker : Mocker<HttpMessageHandler>
{
    private const string SEND_ASYNC_METHOD = "SendAsync";

    /// <summary>
    ///     Configures the mock to return the specified HTTP response message
    ///     when SendAsync is called.
    /// </summary>
    /// <param name="returnValue">
    ///     The HTTP response message to return from SendAsync. Cannot be null.
    /// </param>
    /// <returns>
    ///     The current <see cref="HttpMessageHandlerMocker"/> instance,
    ///     enabling method chaining.
    /// </returns>
    public HttpMessageHandlerMocker WithSendAsync(HttpResponseMessage returnValue)
    {
        MockInstance
            .Protected()
            .Setup<Task<HttpResponseMessage>>(SEND_ASYNC_METHOD, ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(returnValue);

        return this;
    }

    /// <summary>
    ///     Configures the mock to return an HTTP 200 OK response with the
    ///     specified object as a JSON content for any request.
    /// </summary>
    /// <param name="returnValue">
    ///     The object that will be serialized to a JSON string to include
    ///     as the response content in the mocked HTTP response.
    /// </param>
    /// <returns>
    ///     The current <see cref="HttpMessageHandlerMocker"/> instance,
    ///     enabling method chaining.
    /// </returns>
    public HttpMessageHandlerMocker WithSendAsync(object returnValue)
    {
        var json = JsonSerializer.Serialize(returnValue);
        var content = new StringContent(
            content: json,
            encoding: Encoding.UTF8,
            mediaType: "application/json"
        );

        MockInstance
            .Protected()
            .Setup<Task<HttpResponseMessage>>(SEND_ASYNC_METHOD, ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = content });

        return this;
    }
}
