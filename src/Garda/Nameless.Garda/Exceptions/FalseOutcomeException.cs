using System.Runtime.Serialization;

namespace Nameless {
    [Serializable]
    public class FalseOutcomeException : Exception {
        #region Public Constructors

        public FalseOutcomeException() { }
        public FalseOutcomeException(string message)
            : base(message) { }
        public FalseOutcomeException(string message, Exception inner)
            : base(message, inner) { }

        #endregion

        #region Protected Constructors
        protected FalseOutcomeException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        #endregion
    }
}
