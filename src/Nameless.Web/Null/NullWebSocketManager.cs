using System.Net.WebSockets;
using Microsoft.AspNetCore.Http;

namespace Nameless.Web.Null;

/// <summary>
///     Null implementation of <see cref="WebSocketManager"/> that does not support WebSocket operations.
/// </summary>
public sealed class NullWebSocketManager : WebSocketManager {
    public static WebSocketManager Instance { get; } = new NullWebSocketManager();

    /// <inheritdoc />
    public override bool IsWebSocketRequest => false;

    /// <inheritdoc />
    public override IList<string> WebSocketRequestedProtocols => [];

    static NullWebSocketManager() { }

    private NullWebSocketManager() { }

    /// <inheritdoc />
    public override Task<WebSocket> AcceptWebSocketAsync(string? subProtocol) {
        return Task.FromResult(WebSocket.CreateFromStream(Stream.Null, new WebSocketCreationOptions()));
    }
}