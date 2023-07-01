namespace Nameless.Messenger {
    public sealed class MessageResponse {
        #region Public Properties

        public Exception? Error { get; }
        public bool Success => Error == null;

        #endregion

        #region Private Constructors

        private MessageResponse() { }

        private MessageResponse(Exception error) {
            Garda.Prevent.Null(error, nameof(error));

            Error = error;
        }

        #endregion

        #region Public Static Methods

        public static MessageResponse Successful() => new();
        public static MessageResponse Failure(Exception error) => new(error);
        public static MessageResponse Failure(string message) => new(new(message));

        #endregion
    }
}
