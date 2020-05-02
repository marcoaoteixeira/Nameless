using System;
using System.Runtime.Serialization;

namespace Nameless.WebApplication.Identity {
    [Serializable]
    public class UserNotFoundException : Exception {
        #region Public Constructors

        public UserNotFoundException () { }
        public UserNotFoundException (string message) : base (message) { }
        public UserNotFoundException (string message, Exception inner) : base (message, inner) { }

        #endregion

        #region Protected Constructors

        protected UserNotFoundException (SerializationInfo info, StreamingContext context) : base (info, context) { }

        #endregion
    }
}