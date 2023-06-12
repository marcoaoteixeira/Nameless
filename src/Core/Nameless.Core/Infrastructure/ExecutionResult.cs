namespace Nameless.Infrastructure {

    public sealed class ExecutionResult {

        #region Public Read-Only Properties

        public object? State { get; }

        public Error[] Errors { get; } = Array.Empty<Error>();

        public bool Success => Errors.IsNullOrEmpty();

        #endregion

        #region Private Constructors

        // block object initialization from outside.
        private ExecutionResult(object? state) => State = state;

        private ExecutionResult(Error[] errors) => Errors = errors.IsNullOrEmpty()
            ? new[] { new Error("ERROR", "ERROR") }
            : errors;

        #endregion

        #region Public Static Methods

        public static ExecutionResult Successful(object? state = default) => new(state);

        public static ExecutionResult Failure(params Error[] errors) => new(errors);

        public static ExecutionResult Failure(string reason) => new(new[] { new Error("reason", reason) });

        #endregion

        #region Public Methods

        public T? GetStateAs<T>() => (T?)State;

        #endregion

        #region Records

        public record Error(string Property, string Message);

        #endregion
    }
}
