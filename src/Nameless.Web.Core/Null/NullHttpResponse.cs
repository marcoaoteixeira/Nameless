using Microsoft.AspNetCore.Http;

namespace Nameless.Web.Null;

/// <summary>
///     Null implementation of <see cref="HttpResponse"/> that does not perform any operations.
/// </summary>
public sealed class NullHttpResponse : HttpResponse {
    public static HttpResponse Instance { get; } = new NullHttpResponse();

    /// <inheritdoc />
    public override HttpContext HttpContext => NullHttpContext.Instance;

    /// <inheritdoc />
    public override int StatusCode {
        get => 0;
        set => _ = value;
    }

    /// <inheritdoc />
    public override IHeaderDictionary Headers => NullHeaderDictionary.Instance;

    /// <inheritdoc />
    public override Stream Body {
        get => Stream.Null;
        set => _ = value;
    }

    /// <inheritdoc />
    public override long? ContentLength {
        get => null;
        set => _ = value;
    }

    /// <inheritdoc />
    public override string? ContentType {
        get => null;
        set => _ = value;
    }

    /// <inheritdoc />
    public override IResponseCookies Cookies => NullResponseCookies.Instance;

    /// <inheritdoc />
    public override bool HasStarted => false;

    static NullHttpResponse() { }

    private NullHttpResponse() { }

    /// <inheritdoc />
    public override void OnStarting(Func<object, Task> callback, object state) { }

    /// <inheritdoc />
    public override void OnCompleted(Func<object, Task> callback, object state) { }

    /// <inheritdoc />
    public override void Redirect(string location, bool permanent) { }
}