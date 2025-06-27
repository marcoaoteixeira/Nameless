using Microsoft.AspNetCore.Http;

namespace Nameless.Web.Null;

/// <summary>
///     Null implementation of <see cref="HttpRequest"/> that does not perform any operations.
/// </summary>
public sealed class NullHttpRequest : HttpRequest {
    public static HttpRequest Instance { get; } = new NullHttpRequest();

    /// <inheritdoc />
    public override HttpContext HttpContext => NullHttpContext.Instance;

    /// <inheritdoc />
    public override string Method {
        get => string.Empty;
        set => _ = value;
    }

    /// <inheritdoc />
    public override string Scheme {
        get => string.Empty;
        set => _ = value;
    }

    /// <inheritdoc />
    public override bool IsHttps {
        get => false;
        set => _ = value;
    }

    /// <inheritdoc />
    public override HostString Host {
        get => default;
        set => _ = value;
    }

    /// <inheritdoc />
    public override PathString PathBase {
        get => default;
        set => _ = value;
    }

    /// <inheritdoc />
    public override PathString Path {
        get => default;
        set => _ = value;
    }

    /// <inheritdoc />
    public override QueryString QueryString {
        get => default;
        set => _ = value;
    }

    /// <inheritdoc />
    public override IQueryCollection Query {
        get => NullQueryCollection.Instance;
        set => _ = value;
    }

    /// <inheritdoc />
    public override string Protocol {
        get => string.Empty;
        set => _ = value;
    }

    /// <inheritdoc />
    public override IHeaderDictionary Headers => NullHeaderDictionary.Instance;

    /// <inheritdoc />
    public override IRequestCookieCollection Cookies {
        get => NullRequestCookieCollection.Instance;
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
    public override Stream Body {
        get => Stream.Null;
        set => _ = value;
    }

    /// <inheritdoc />
    public override bool HasFormContentType => false;

    /// <inheritdoc />
    public override IFormCollection Form {
        get => NullFormCollection.Instance;
        set => _ = value;
    }

    static NullHttpRequest() { }

    private NullHttpRequest() { }

    /// <inheritdoc />
    public override Task<IFormCollection> ReadFormAsync(CancellationToken cancellationToken = default) {
        return Task.FromResult(NullFormCollection.Instance);
    }
}