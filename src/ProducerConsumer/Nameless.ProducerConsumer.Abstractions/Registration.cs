using System.Reflection;

namespace Nameless.ProducerConsumer {
    /// <summary>
    /// Represents a consumer registration, also holds the reference to the callback method.
    /// </summary>
    public sealed class Registration<T> : IDisposable {

        #region Private Fields

        private MethodInfo _methodInfo;
        private WeakReference _targetObject;
        private bool _isStatic;
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

        /// <summary>
        /// Gets the info about the method that will handle the message.
        /// </summary>
        public MemberInfo Callback => _methodInfo;

        #endregion

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="Subscription"/>.
        /// </summary>
        /// <param name="callback">The message handler.</param>
        /// <param name="tag">The registration tag.</param>
        public Registration(string tag, string topic, MessageEventHandler<T> handler) {
            Prevent.NullOrWhiteSpaces(tag, nameof(tag));
            Prevent.NullOrWhiteSpaces(topic, nameof(topic));
            Prevent.Null(handler, nameof(handler));

            Tag = tag;
            Topic = topic;

            _methodInfo = handler.Method;
            _targetObject = new WeakReference(handler.Target);
            _isStatic = handler.Target == default;
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
        /// <returns>An instance of <see cref="MessageEventHandler{T}" />.</returns>
        public MessageEventHandler<T>? CreateHandler() {
            BlockAccessAfterDispose();

            if (_targetObject.Target != default && _targetObject.IsAlive) {
                return (MessageEventHandler<T>)_methodInfo.CreateDelegate(typeof(MessageEventHandler<T>), _targetObject.Target);
            }

            if (_isStatic) {
                return (MessageEventHandler<T>)_methodInfo.CreateDelegate(typeof(MessageEventHandler<T>));
            }

            return default;
        }

        #endregion

        #region Private Methods

        private void BlockAccessAfterDispose() {
            if (_disposed) {
                throw new ObjectDisposedException(nameof(Registration<T>));
            }
        }

        private void Dispose(bool disposing) {
            if (_disposed) { return; }
            if (disposing) { /* Dispose managed resources */ }
            // Dispose unmanaged resources

            _methodInfo = default!;
            _targetObject = default!;
            _isStatic = false;

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
