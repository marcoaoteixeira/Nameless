using System;

namespace Nameless.CQRS {
    public sealed class ExecutionResult {
        #region Public Properties

        public bool Succeeded => string.IsNullOrWhiteSpace (ErrorMessage);

        public dynamic State { get; }

        public string ErrorMessage { get; }

        #endregion

        #region Public Constructors

        public ExecutionResult (dynamic state = null, string errorMessage = null) {
            State = state;
            ErrorMessage = errorMessage;
        }

        #endregion

        #region Public Static Methods

        public static ExecutionResult Success (dynamic state = null) {
            return new ExecutionResult (state: state);
        }

        public static ExecutionResult Failure (string errorMessage) {
            if (string.IsNullOrWhiteSpace (errorMessage)) {
                throw new ArgumentException ("Parameter cannot be null, empty or white spaces.", nameof (errorMessage));
            }

            return new ExecutionResult (errorMessage: errorMessage);
        }

        #endregion
    }
}