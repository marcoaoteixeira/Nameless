using System.Reflection;

namespace Nameless.ProducerConsumer {
    /// <summary>
    /// Represents a consumer registration, also holds the reference to the callback method.
    /// </summary>
    public sealed class Registration<T> : IDisposable {
        #region Private Fields

        private MethodInfo _handler;
        private WeakReference _target;
        private bool _staticHandler;
        private bool _disposed;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the registration's tag.
        /// </summary>
        public string Tag { get; }

        /// <summary>
        /// Gets the topic.
        /// </summary>
        public string Topic { get; }

        #endregion

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="Subscription"/>.
        /// </summary>
        /// <param name="callback">The message handler.</param>
        /// <param name="tag">The registration tag.</param>
        public Registration(string tag, string topic, MessageHandler<T> handler) {
            Guard.Against.Null(handler, nameof(handler));

            Tag = Guard.Against.NullOrWhiteSpace(tag, nameof(tag));
            Topic = Guard.Against.NullOrWhiteSpace(topic, nameof(topic));

            _handler = handler.Method;
            _target = new WeakReference(handler.Target);
            _staticHandler = handler.Target is null;
        }

        #endregion

        #region Destructor

        /// <summary>
        /// Destructor
        /// </summary>
        ~Registration()
            => Dispose(disposing: false);

        #endregion

        #region Public Override Methods

        public override string ToString()
            => $"[{Topic};{Tag}]";

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a handler for the subscription.
        /// </summary>
        /// <returns>An instance of <see cref="MessageHandler{T}" />.</returns>
        public MessageHandler<T>? CreateHandler() {
            BlockAccessAfterDispose();

            if (_target.Target is not null && _target.IsAlive) {
                return (MessageHandler<T>)_handler.CreateDelegate(typeof(MessageHandler<T>), _target.Target);
            }

            if (_staticHandler) {
                return (MessageHandler<T>)_handler.CreateDelegate(typeof(MessageHandler<T>));
            }

            return null;
        }

        #endregion

        #region Private Methods

        private void BlockAccessAfterDispose()
            => ObjectDisposedException.ThrowIf(_disposed, typeof(Registration<>));

        private void Dispose(bool disposing) {
            if (_disposed) { return; }
            if (disposing) { /* Dispose managed resources */ }
            // Dispose unmanaged resources

            _handler = null!;
            _target = null!;
            _staticHandler = false;

            _disposed = true;
        }

        #endregion

        #region IDisposable Members

        /// <inheritdoc />
        void IDisposable.Dispose() {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
