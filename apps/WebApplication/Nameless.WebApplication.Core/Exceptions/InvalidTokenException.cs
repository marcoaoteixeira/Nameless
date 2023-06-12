using System.Runtime.Serialization;

namespace Nameless.WebApplication {


    [Serializable]
    public class InvalidTokenException : Exception {

        #region Public Constructors

        public InvalidTokenException() { }
        
        public InvalidTokenException(string message)
            : base(message) { }

        public InvalidTokenException(string message, Exception inner)
            : base(message, inner) { }

        #endregion

        #region Protected Constructors

        protected InvalidTokenException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        #endregion
    }
}
