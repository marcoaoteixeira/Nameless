namespace Nameless.Infrastructure {
    public sealed record Arg {
        #region Public Properties

        public string Name { get; }
        public object Value { get; }

        #endregion

        #region Public Constructors

        public Arg(string name, object value) {
            Name = Guard.Against.NullOrWhiteSpace(name, nameof(name));
            Value = Guard.Against.Null(value, nameof(value));
        }

        #endregion

        #region Public Override Methods

        public override string ToString()
            => $"[{Name}] {Value} ({Value.GetType().Name})";

        #endregion
    }
}
