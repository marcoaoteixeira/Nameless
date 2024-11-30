using System.Diagnostics;
using System.Reflection;

namespace Nameless.ProducerConsumer;

/// <summary>
/// Represents a consumer registration, also holds the reference to the callback method.
/// </summary>
[DebuggerDisplay("{DebuggerDisplayValue,nq}")]
public sealed class Registration<TMessage> : IDisposable {
    private readonly bool _isStatic;

    private MethodInfo? _method;
    private WeakReference? _ref;
    private bool _disposed;

    private string DebuggerDisplayValue
        => $"{nameof(Tag)}: {Tag}, {nameof(Topic)}: {Topic}";

    /// <summary>
    /// Gets the registration's tag.
    /// </summary>
    public string Tag { get; }

    /// <summary>
    /// Gets the topic.
    /// </summary>
    public string Topic { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="Registration{T}"/>.
    /// </summary>
    /// <param name="tag">The registration tag.</param>
    /// <param name="topic">The topic.</param>
    /// <param name="messageHandler">The message handler.</param>
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="tag"/> or
    /// <paramref name="topic"/> or
    /// <paramref name="messageHandler"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// if <paramref name="tag"/> is empty or white spaces.
    /// </exception>
    public Registration(string tag,
                        string topic,
                        MessageHandlerAsync<TMessage> messageHandler) {
        Prevent.Argument.Null(messageHandler);

        Tag = Prevent.Argument.NullOrWhiteSpace(tag);
        Topic = Prevent.Argument.Null(topic);

        _method = messageHandler.Method;
        _ref = new WeakReference(messageHandler.Target);
        _isStatic = messageHandler.Target is null;
    }

    ~Registration() {
        Dispose(disposing: false);
    }

    /// <summary>
    /// Creates a handler for the subscription.
    /// </summary>
    /// <returns>An instance of <see cref="MessageHandlerAsync{TMessage}" />.</returns>
    public MessageHandlerAsync<TMessage>? CreateMessageHandler() {
        BlockAccessAfterDispose();

#if NET8_0_OR_GREATER
        if (GetRef().Target is not null && GetRef().IsAlive) {
            return GetMethod().CreateDelegate<MessageHandlerAsync<TMessage>>(target: GetRef().Target);
        }

        return _isStatic
            ? GetMethod().CreateDelegate<MessageHandlerAsync<TMessage>>()
            : null;
#else
        if (GetRef().Target is not null && GetRef().IsAlive) {
            return (MessageHandlerAsync<TMessage>)GetMethod().CreateDelegate(delegateType: typeof(MessageHandlerAsync<TMessage>),
                                                                             target: GetRef().Target);
        }

        if (_isStatic) {
            return (MessageHandlerAsync<TMessage>)GetMethod().CreateDelegate(delegateType: typeof(MessageHandlerAsync<TMessage>));
        }

        return null;
#endif
    }

    /// <inheritdoc />
    public void Dispose() {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public override string ToString() => Tag;

    private MethodInfo GetMethod()
        => _method ?? throw new ArgumentNullException(nameof(_method));

    private WeakReference GetRef()
        => _ref ?? throw new ArgumentNullException(nameof(_ref));

    private void BlockAccessAfterDispose() {
#if NET8_0_OR_GREATER
        ObjectDisposedException.ThrowIf(_disposed, this);
#else
        if (_disposed) {
            throw new ObjectDisposedException(GetType().Name);
        }
#endif
    }

    private void Dispose(bool disposing) {
        if (_disposed) { return; }
        if (disposing) { }

        _method = null;
        _ref = null;

        _disposed = true;
    }
}