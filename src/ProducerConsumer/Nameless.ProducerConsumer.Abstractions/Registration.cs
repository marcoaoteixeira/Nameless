using System.Reflection;
using Nameless.Helpers;

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
        public Registration(string tag, string topic, Action<T> callback) {
            Prevent.NullOrWhiteSpaces(tag, nameof(tag));
            Prevent.NullOrWhiteSpaces(topic, nameof(topic));
            Prevent.Null(callback, nameof(callback));

            Tag = tag;
            Topic = topic;

            _methodInfo = callback.Method;
            _targetObject = new WeakReference(callback.Target);
            _isStatic = callback.Target == default;
        }

        #endregion

        #region Destructor

        /// <summary>
        /// Destructor
        /// </summary>
        ~Registration() => Dispose(disposing: false);

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a handler for the subscription.
        /// </summary>
        /// <returns>An instance of <see cref="Action{object}" />.</returns>
        public Action<T>? CreateHandler() {
            BlockAccessAfterDispose();

            if (_targetObject.Target != default && _targetObject.IsAlive) {
                return (Action<T>)_methodInfo.CreateDelegate(typeof(Action<T>), _targetObject.Target);
            }

            if (_isStatic) {
                return (Action<T>)_methodInfo.CreateDelegate(typeof(Action<T>));
            }

            return default;
        }

        /// <summary>
        /// Checks for equality of the object.
        /// </summary>
        /// <param name="obj">The other <see cref="Subscription" /> object.</param>
        /// <returns><c>true</c> if equals, otherwise <c>false</c>.</returns>
        public bool Equals(Registration<T>? obj) => obj != default
            && obj.Tag == Tag
            && obj.Topic == Topic
            && obj.Callback == Callback;

        #endregion

        #region Public Override Methods

        /// <inheritdoc />
        public override bool Equals(object? obj) => Equals(obj as Registration<T>);

        /// <inheritdoc />
        public override int GetHashCode() => SimpleHash.Compute(Tag, Topic, Callback);

        /// <inheritdoc />
        public override string ToString() {
            return $"[{Topic}] Handler: {Tag}";
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
        public void Dispose() {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}