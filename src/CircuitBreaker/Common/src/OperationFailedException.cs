using System;
using System.Runtime.Serialization;

namespace Nameless.CircuitBreaker {

    [Serializable]
    public class OperationFailedException : Exception {
        #region Public Constructors

        public OperationFailedException (string message) : base (message) { }
        public OperationFailedException (string message, Exception inner) : base (message, inner) { }
        
        #endregion

        #region Protected Constructors

        protected OperationFailedException (SerializationInfo info, StreamingContext context) : base (info, context) { }
        
        #endregion
    }
}
