using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Nameless.PubSub;

/// <summary>
/// Represents a subscriber's subscription.
/// </summary>
[DebuggerDisplay("{DebuggerDisplayValue,nq}")]
public sealed class Subscription : IDisposable {
    private readonly bool _isStatic;

    private WeakReference<MethodInfo>? _methodRef;
    private WeakReference? _targetRef;

    private bool _disposed;

    private string DebuggerDisplayValue
        => $"[{Topic}]: {ID}";

    public string ID { get; }

    /// <summary>
    /// Gets the topic.
    /// </summary>
    public string Topic { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="Subscription"/>.
    /// </summary>
    /// <param name="topic">The topic.</param>
    /// <param name="messageHandler">The message handler.</param>
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="topic"/> or
    /// <paramref name="messageHandler"/> is <c>null</c>.
    /// </exception>
    public Subscription(string topic, Func<object, CancellationToken, Task> messageHandler) {
        Prevent.Argument.Null(messageHandler);

        ID = Guid.NewGuid().ToString("N");
        Topic = Prevent.Argument.Null(topic);

        _isStatic = messageHandler.Target is null;

        _methodRef = new WeakReference<MethodInfo>(messageHandler.Method);
        _targetRef = new WeakReference(messageHandler.Target);
    }

    ~Subscription() {
        Dispose(disposing: false);
    }

    /// <summary>
    /// Creates a handler for the subscription.
    /// </summary>
    /// <returns>An instance of <see cref="MessageHandlerAsync" />.</returns>
    public MessageHandlerAsync? CreateMessageHandler() {
        BlockAccessAfterDispose();

        if (TryGetTarget(out var target) && TryGetMethod(out var method)) {
#if NET8_0_OR_GREATER
            return method.CreateDelegate<MessageHandlerAsync>(target);
#else
            return (MessageHandlerAsync)method.CreateDelegate(typeof(MessageHandlerAsync), target);
#endif
        }

        if (_isStatic && TryGetMethod(out method)) {
#if NET8_0_OR_GREATER
            return method.CreateDelegate<MessageHandlerAsync>();
#else
            return (MessageHandlerAsync)method.CreateDelegate(typeof(MessageHandlerAsync));
#endif
        }

        return null;
    }

    public void Dispose() {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing) {
        if (_disposed) { return; }

        if (disposing) { }

        _methodRef = null;
        _targetRef = null;

        _disposed = true;
    }

    private void BlockAccessAfterDispose() {
#if NET8_0_OR_GREATER
        ObjectDisposedException.ThrowIf(_disposed, this);
#else
        if (_disposed) {
            throw new ObjectDisposedException(nameof(Subscription));
        }
#endif
    }

    private bool TryGetTarget([NotNullWhen(returnValue: true)] out object? target) {
        target = null;

        if (_targetRef is { Target: not null, IsAlive: true }) {
            target = _targetRef.Target;
        }

        return target is not null;
    }

    private bool TryGetMethod([NotNullWhen(returnValue: true)] out MethodInfo? method) {
        method = null;

        return _methodRef is not null && _methodRef.TryGetTarget(out method);
    }
}