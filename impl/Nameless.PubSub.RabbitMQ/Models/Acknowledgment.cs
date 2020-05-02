using System;

namespace Nameless.PubSub.RabbitMQ {
    public class Acknowledgment {
        #region Public Properties

        public object Request { get; }
        public ExceptionInfo Exception { get; }
        public DateTimeOffset Date { get; }

        #endregion

        #region Public Constructors

        public Acknowledgment (object request, Exception ex = null) {
            Prevent.ParameterNull (request, nameof (request));

            Request = request;
            Exception = ExceptionInfo.Create(ex);
            Date = DateTimeOffset.Now;
        }

        #endregion

        #region Public Static Methods

        public static Acknowledgment Create (object request, Exception ex) {
            return new Acknowledgment (request, ex);
        }

        #endregion
    }

    public class ExceptionInfo {
        #region Public Properties

        public string Type { get; }
        public string Message { get; }
        public ExceptionInfo Inner { get; }

        #endregion

        #region Public Constructors

        public ExceptionInfo (string type, string message, ExceptionInfo inner = null) {
            Prevent.ParameterNullOrWhiteSpace (type, nameof (type));
            
            Type = type;
            Message = message;
            Inner = inner;
        }

        #endregion

        #region Public Static Methods

        public static ExceptionInfo Create (Exception ex) {
            if (ex == null) { return null; }
            return new ExceptionInfo (ex.GetType ().FullName, ex.Message, Create (ex.InnerException));
        }

        #endregion
    }
}