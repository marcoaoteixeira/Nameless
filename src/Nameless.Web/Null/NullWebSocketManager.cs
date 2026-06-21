using System.Diagnostics.CodeAnalysis;
using System.Net.WebSockets;
using Microsoft.AspNetCore.Http;
using Nameless.Diagnostics.CodeAnalysis;
using Nameless.Registration;

namespace Nameless.Web.Null;

/// <summary>
///     Null implementation of <see cref="WebSocketManager"/> that does not support WebSocket operations.
/// </summary>
[ExcludeFromCodeCoverage(Justification = CodeCoverage.Justifications.TrivialCode)]
[IgnoreAssemblyScan]
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