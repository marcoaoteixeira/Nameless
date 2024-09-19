using System.Diagnostics;
using System.Reflection;

namespace Nameless.ProducerConsumer;

/// <summary>
/// Represents a consumer registration, also holds the reference to the callback method.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public sealed class Registration<T> : IDisposable {
    private readonly bool _isStatic;

    private MethodInfo? _method;
    private WeakReference? _ref;
    private bool _disposed;

    private string DebuggerDisplay => $"{{ \"Topic\": \"{Topic}\", \"Tag\": \"{Tag}\" }}";

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
    /// <param name="handler">The message handler.</param>
    public Registration(string tag, string topic, MessageHandler<T> handler) {
        Prevent.Argument.Null(handler);

        Tag = Prevent.Argument.NullOrWhiteSpace(tag);
        Topic = Prevent.Argument.Null(topic);

        _method = handler.Method;
        _ref = new WeakReference(handler.Target);
        _isStatic = handler.Target is null;
    }

    ~Registration() {
        Dispose(disposing: false);
    }

    /// <summary>
    /// Creates a handler for the subscription.
    /// </summary>
    /// <returns>An instance of <see cref="MessageHandler{T}" />.</returns>
    public MessageHandler<T>? CreateHandler() {
        BlockAccessAfterDispose();

        if (GetRef().Target is not null && GetRef().IsAlive) {
            return (MessageHandler<T>)GetMethod().CreateDelegate(delegateType: typeof(MessageHandler<T>),
                                                                 target: GetRef().Target);
        }

        if (_isStatic) {
            return (MessageHandler<T>)GetMethod().CreateDelegate(delegateType: typeof(MessageHandler<T>));
        }

        return null;
    }

    /// <inheritdoc />
    void IDisposable.Dispose() {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private MethodInfo GetMethod()
        => _method ?? throw new ArgumentNullException(nameof(_method));

    private WeakReference GetRef()
        => _ref ?? throw new ArgumentNullException(nameof(_ref));

    private void BlockAccessAfterDispose() {
        if (_disposed) {
            throw new ObjectDisposedException(typeof(Registration<>).Name);
        }
    }

    private void Dispose(bool disposing) {
        if (_disposed) { return; }
        if (disposing) { }

        _method = null;
        _ref = null;

        _disposed = true;
    }
}