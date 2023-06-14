namespace Nameless.ErrorHandling {
    public sealed class ErrorHandlerException : Exception {

        #region Public Constructors

        public ErrorHandlerException(Exception inner)
            : base("Exception thrown by ErrorHandling system. See inner exception for more info.", inner) { }

        #endregion

    }
}
