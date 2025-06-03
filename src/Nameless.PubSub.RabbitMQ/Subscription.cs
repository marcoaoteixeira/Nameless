using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Nameless.PubSub.RabbitMQ;

/// <summary>
///     Represents a subscriber's subscription.
/// </summary>
[DebuggerDisplay("{DebuggerDisplayValue,nq}")]
public sealed class Subscription : IDisposable {
    private readonly bool _isStatic;

    private bool _disposed;

    private WeakReference<MethodInfo>? _methodRef;
    private WeakReference? _targetRef;

    private string DebuggerDisplayValue => $"[{Topic}]: {ConsumerTag}";

    /// <summary>
    /// Gets the consumer tag of the subscription.
    /// </summary>
    public string ConsumerTag { get; }

    /// <summary>
    ///     Gets the topic.
    /// </summary>
    public string Topic { get; }

    /// <summary>
    ///     Initializes a new instance of <see cref="Subscription" />.
    /// </summary>
    /// <param name="topic">The topic.</param>
    /// <param name="messageHandler">The message handler.</param>
    /// <exception cref="ArgumentNullException">
    ///     if <paramref name="topic" /> or
    ///     <paramref name="messageHandler" /> is <c>null</c>.
    /// </exception>
    public Subscription(string topic, MessageHandlerDelegate messageHandler) {
        Prevent.Argument.Null(messageHandler);

        ConsumerTag = Guid.NewGuid().ToString("N");
        Topic = Prevent.Argument.Null(topic);

        _isStatic = messageHandler.Target is null;
        _methodRef = new WeakReference<MethodInfo>(messageHandler.Method);
        _targetRef = new WeakReference(messageHandler.Target);
    }

    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~Subscription() {
        Dispose(false);
    }

    /// <summary>
    ///     Creates a handler for the subscription.
    /// </summary>
    /// <returns>An instance of <see cref="MessageHandlerDelegate" />.</returns>
    public MessageHandlerDelegate? CreateMessageHandler() {
        BlockAccessAfterDispose();

        if (TryGetTarget(out var target) && TryGetMethod(out var method)) {
            return method.CreateDelegate<MessageHandlerDelegate>(target);
        }

        if (_isStatic && TryGetMethod(out method)) {
            return method.CreateDelegate<MessageHandlerDelegate>();
        }

        return null;
    }

    private void Dispose(bool disposing) {
        if (_disposed) { return; }

        if (disposing) { }

        _methodRef = null;
        _targetRef = null;

        _disposed = true;
    }

    private void BlockAccessAfterDispose() {
        ObjectDisposedException.ThrowIf(_disposed, this);
    }

    private bool TryGetTarget([NotNullWhen(true)] out object? target) {
        target = null;

        if (_targetRef is { Target: not null, IsAlive: true }) {
            target = _targetRef.Target;
        }

        return target is not null;
    }

    private bool TryGetMethod([NotNullWhen(true)] out MethodInfo? method) {
        method = null;

        return _methodRef is not null && _methodRef.TryGetTarget(out method);
    }
}