using System.Diagnostics;

namespace Nameless.Infrastructure {
    [DebuggerDisplay("{DebuggerDisplay}")]
    public sealed record Arg {
        #region Private Properties

        private string DebuggerDisplay
            => $"[{Name}] {Value} ({Value.GetType().Name})";

        #endregion

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
    }
}
