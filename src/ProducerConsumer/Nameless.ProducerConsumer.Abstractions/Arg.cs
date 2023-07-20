namespace Nameless.ProducerConsumer {
    public sealed record Arg {
        #region Public Properties

        public string Name { get; }
        public object Value { get; }

        #endregion

        #region Public Constructors

        public Arg(string name, object value) {
            Prevent.Against.NullOrWhiteSpace(name, nameof(name));
            Prevent.Against.Null(value, nameof(value));

            Name = name;
            Value = value;
        }

        #endregion

        #region Public Override Methods

        public override string ToString() {
            return $"[{Name}] {Value} ({Value.GetType().Name})";
        }

        #endregion
    }
}
