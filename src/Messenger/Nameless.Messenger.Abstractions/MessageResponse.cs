namespace Nameless.Messenger {
    public sealed record MessageResponse {
        #region Public Properties

        public Exception? Error { get; set; }
        public bool Succeeded => Error is null;

        #endregion

        #region Private Constructors

        private MessageResponse(Exception? error = null) {
            Error = error;
        }

        #endregion

        #region Public Static Methods

        public static MessageResponse Success() => new();
        public static MessageResponse Failure(Exception error) => new(error);
        public static MessageResponse Failure(string message) => new(new Exception(message));

        #endregion
    }
}
