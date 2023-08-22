namespace Nameless.Messenger {
    public sealed record Response {
        #region Public Properties

        public Exception? Error { get; init; }
        public bool Success => Error is null;

        #endregion

        #region Private Constructors

        private Response(Exception? error = null) {
            Error = error;
        }

        #endregion

        #region Public Static Methods

        public static Response Successful() => new();
        public static Response Failure(Exception error) => new(error);
        public static Response Failure(string message) => new(new Exception(message));

        #endregion
    }
}
