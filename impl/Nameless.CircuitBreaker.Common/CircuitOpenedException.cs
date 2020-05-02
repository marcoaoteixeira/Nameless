using System;
using System.Runtime.Serialization;

namespace Nameless.CircuitBreaker.Common {

    [Serializable]
    public class CircuitOpenedException : Exception {
        #region Public Constructors

        public CircuitOpenedException () { }
        public CircuitOpenedException (string message) : base (message) { }
        public CircuitOpenedException (string message, Exception inner) : base (message, inner) { }

        #endregion

        #region Protected Constructors

        protected CircuitOpenedException (SerializationInfo info, StreamingContext context) : base (info, context) { }

        #endregion
    }
}
