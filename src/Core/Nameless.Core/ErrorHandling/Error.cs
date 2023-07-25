namespace Nameless.ErrorHandling {
    public sealed record Error {
        #region Public Properties

        public string Code { get; }
        public string[] Problems { get; }

        #endregion

        #region Public Constructors

        public Error(string code, string[] problems) {
            Code = Prevent.Against.Null(code, nameof(code));
            Problems = Prevent.Against.Null(problems, nameof(problems));
        }

        #endregion
    }
}
