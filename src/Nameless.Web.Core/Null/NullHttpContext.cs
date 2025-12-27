using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Nameless.Null;

namespace Nameless.Web.Null;

/// <summary>
///     Null implementation of <see cref="HttpContext"/> that does not store any request or response data.
/// </summary>
public sealed class NullHttpContext : HttpContext {
    public static HttpContext Instance { get; } = new NullHttpContext();

    /// <inheritdoc />
    public override IFeatureCollection Features => NullFeatureCollection.Instance;

    /// <inheritdoc />
    public override HttpRequest Request => NullHttpRequest.Instance;

    /// <inheritdoc />
    public override HttpResponse Response => NullHttpResponse.Instance;

    /// <inheritdoc />
    public override ConnectionInfo Connection => NullConnectionInfo.Instance;

    /// <inheritdoc />
    public override WebSocketManager WebSockets => NullWebSocketManager.Instance;

    /// <inheritdoc />
    public override ClaimsPrincipal User {
        get => new();
        set => _ = value;
    }

    /// <inheritdoc />
    public override IDictionary<object, object?> Items {
        get => NullDictionary<object, object?>.Instance;
        set => _ = value;
    }

    /// <inheritdoc />
    public override IServiceProvider RequestServices {
        get => NullServiceProvider.Instance;
        set => _ = value;
    }

    /// <inheritdoc />
    public override CancellationToken RequestAborted {
        get => CancellationToken.None;
        set => _ = value;
    }

    /// <inheritdoc />
    public override string TraceIdentifier {
        get => string.Empty;
        set => _ = value;
    }

    /// <inheritdoc />
    public override ISession Session {
        get => NullSession.Instance;
        set => _ = value;
    }

    static NullHttpContext() { }

    private NullHttpContext() { }

    /// <inheritdoc />
    public override void Abort() { }
}