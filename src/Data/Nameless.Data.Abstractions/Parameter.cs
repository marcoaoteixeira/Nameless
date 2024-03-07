using System.Data;
using System.Diagnostics;

namespace Nameless.Data {
    [DebuggerDisplay("{DebuggerDisplayValue}")]
    public sealed record Parameter {
        #region Public Properties

        public string Name { get; }
        public object? Value { get; }
        public DbType Type { get; }

        #endregion

        #region Private Properties

        private string DebuggerDisplayValue
            => $"[{Type}] {Name} => {Value ?? "NULL"}";

        #endregion

        #region Public Constructors

        public Parameter(string name, object? value, DbType type = DbType.String) {
            Name = Guard.Against.NullOrWhiteSpace(name, nameof(name));
            Value = value;
            Type = type;
        }

        #endregion
    }
}