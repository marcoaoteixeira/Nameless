using Microsoft.AspNetCore.Http;

namespace Nameless.Testing.Tools.Mockers;

public sealed class HttpContextAccessorMocker : Mocker<IHttpContextAccessor> {
    public HttpContextAccessorMocker WithHttpContext(HttpContext returnValue) {
        MockInstance
           .Setup(mock => mock.HttpContext)
           .Returns(returnValue);

        return this;
    }
}