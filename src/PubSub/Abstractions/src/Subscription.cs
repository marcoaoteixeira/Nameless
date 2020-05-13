using System;
using System.Reflection;

namespace Nameless.PubSub {

    public sealed class Subscription : IDisposable {
        #region Private Fields
        
        private MethodInfo _methodInfo;
        private WeakReference _targetObject;
        private bool _isStatic;
        private bool _disposed;

        #endregion

        #region Public Properties

        public string Topic { get; }
        
        public MemberInfo HandlerInfo {
            get { return _methodInfo; }
        }
                
        #endregion

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="Subscription"/>.
        /// </summary>
        /// <param name="topic">The subscription topic.</param>
        /// <param name="handler">The message handler.</param>
        public Subscription (string topic, Action<Message> handler) {
            Prevent.ParameterNullOrWhiteSpace (topic, nameof (topic));
            Prevent.ParameterNull (handler, nameof (handler));

            Topic = topic;
            _methodInfo = handler.GetMethodInfo ();
            _targetObject = new WeakReference (handler.Target);
            _isStatic = handler.Target == null;
        }

        #endregion

        #region Destructor

        /// <summary>
        /// Destructor
        /// </summary>
        ~Subscription () {
            Dispose (disposing: false);
        }

        #endregion

        #region Public Methods

        public Action<Message> CreateHandler () {
            BlockAccessAfterDispose ();

            if (_targetObject.Target != null && _targetObject.IsAlive) {
                return (Action<Message>) _methodInfo.CreateDelegate (typeof (Action<Message>), _targetObject.Target);
            }

            if (_isStatic) {
                return (Action<Message>) _methodInfo.CreateDelegate (typeof (Action<Message>));
            }

            return null;
        }

        public bool Equals (Subscription obj) {
            return obj != null &&
                   obj.Topic == Topic &&
                   obj.HandlerInfo == HandlerInfo;
        }

        #endregion

        #region Public Override Methods

        public override bool Equals (object obj) {
            return Equals (obj as Subscription);
        }

        public override int GetHashCode() {
            var hash = 13;
            unchecked {
                hash += (Topic ?? string.Empty).GetHashCode() * 7;
                hash += (HandlerInfo != null ? HandlerInfo.GetHashCode() : 0) * 7;
            }
            return hash;
        }

        #endregion

        #region Private Methods

        private void BlockAccessAfterDispose () {
            if (_disposed) {
                throw new ObjectDisposedException (GetType ().Name);
            }
        }

        private void Dispose (bool disposing) {
            if (_disposed) { return; }
            if (disposing) { /* Dispose managed resources */ }
            // Dispose unmanaged resources

            _methodInfo = null;
            _targetObject = null;
            _isStatic = false;

            _disposed = true;
        }

        #endregion

        #region IDisposable Members

        /// <inheritdoc />
        public void Dispose () {
            Dispose (disposing: true);
            GC.SuppressFinalize (this);
        }

        #endregion
    }
}