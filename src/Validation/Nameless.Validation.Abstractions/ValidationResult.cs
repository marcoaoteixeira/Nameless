namespace Nameless.Validation.Abstractions {
    public sealed record ValidationResult {
        #region Public Static Read-Only Properties

        public static ValidationResult Empty => new();

        #endregion

        #region Public Properties

        public ValidationEntry[] Errors { get; } = [];

        public bool Succeeded => Errors.Length == 0;

        #endregion

        #region Public Constructors

        public ValidationResult(params ValidationEntry[] errors) {
            Errors = errors;
        }

        #endregion
    }
}