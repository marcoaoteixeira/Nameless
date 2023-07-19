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
            Prevent.Against.NullOrWhiteSpace(tag, nameof(tag));
            Prevent.Against.NullOrWhiteSpace(topic, nameof(topic));
            Prevent.Against.Null(handler, nameof(handler));

            Tag = tag;
            Topic = topic;

            _handler = handler.Method;
            _target = new WeakReference(handler.Target);
            _staticHandler = handler.Target == null;
        }

        #endregion

        #region Destructor

        /// <summary>
        /// Destructor
        /// </summary>
        ~Registration() => Dispose(disposing: false);

        #endregion

        #region Public Override Methods

        public override string ToString() {
            return $"[{Topic};{Tag}]";
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a handler for the subscription.
        /// </summary>
        /// <returns>An instance of <see cref="MessageHandler{T}" />.</returns>
        public MessageHandler<T>? CreateHandler() {
            BlockAccessAfterDispose();

            if (_target.Target != default && _target.IsAlive) {
                return (MessageHandler<T>)_handler.CreateDelegate(typeof(MessageHandler<T>), _target.Target);
            }

            if (_staticHandler) {
                return (MessageHandler<T>)_handler.CreateDelegate(typeof(MessageHandler<T>));
            }

            return default;
        }

        #endregion

        #region Private Methods

        private void BlockAccessAfterDispose() {
            if (_disposed) {
                throw new ObjectDisposedException($"Registration: {Tag}");
            }
        }

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
