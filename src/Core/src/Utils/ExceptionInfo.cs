using System;

namespace Nameless {
    public class ExceptionInfo {
        #region Public Properties

        public string Type { get; }
        public string Message { get; }
        public string StackTrace { get; }
        public ExceptionInfo Inner { get; }

        #endregion

        #region Public Constructors

        public ExceptionInfo (string type, string message, string stackTrace = null, ExceptionInfo inner = null) {
            Prevent.ParameterNullOrWhiteSpace (type, nameof (type));

            Type = type;
            Message = message;
            StackTrace = stackTrace;
            Inner = inner;
        }

        #endregion

        #region Public Static Methods

        public static ExceptionInfo Create (Exception ex) {
            if (ex == null) { return null; }
            return new ExceptionInfo (ex.GetType ().FullName, ex.Message, ex.StackTrace, Create (ex.InnerException));
        }

        #endregion
    }
}