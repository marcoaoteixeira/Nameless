using System.Runtime.Serialization;

namespace Nameless.AspNetCore.Exceptions {

    /// <summary>
    /// Exception for not authenticated access.
    /// </summary>
    [Serializable]
    public class AuthenticationException : Exception {

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="AuthenticationException"/> class.
        /// </summary>
        public AuthenticationException() : this("Authentication credentials are incorrect.") { }
        /// <summary>
        /// Initializes a new instance of <see cref="AuthenticationException"/> class
        /// </summary>
        /// <param name="message">The exception message.</param>
        public AuthenticationException(string message) : base(message) { }
        /// <summary>
        /// Initializes a new instance of <see cref="AuthenticationException"/> class
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="inner">The inner exception.</param>
        public AuthenticationException(string message, Exception inner) : base(message, inner) { }

        #endregion

        #region Protected Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="AuthenticationException"/> class with serialized data.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
        protected AuthenticationException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        #endregion
    }
}
