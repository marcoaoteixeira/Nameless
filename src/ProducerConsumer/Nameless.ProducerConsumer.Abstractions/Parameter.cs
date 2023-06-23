namespace Nameless.ProducerConsumer {

    public record Parameter {

        #region Public Properties

        public string Name { get; }
        public object Value { get; }

        #endregion

        #region Public Constructors

        public Parameter(string name, object value) {
            Prevent.NullOrWhiteSpaces(name, nameof(name));
            Prevent.Null(value, nameof(value));

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
