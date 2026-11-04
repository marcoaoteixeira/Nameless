using System.Net.Http;

namespace Nameless.WinApp.Tests.Tooling;

// A minimal programmable HttpMessageHandler — tests supply the response (or
// throw) for whatever request the client under test sends, without a real
// network call.
internal sealed class FakeHttpMessageHandler : HttpMessageHandler {
    private readonly Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> _handler;

    public FakeHttpMessageHandler(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> handler) {
        _handler = handler;
    }

    public FakeHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> handler)
        : this((request, _) => Task.FromResult(handler(request))) { }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
        return _handler(request, cancellationToken);
    }
}
