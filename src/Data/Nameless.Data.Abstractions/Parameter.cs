using System.Data;

namespace Nameless.Data {
    public sealed record Parameter {
        #region Public Properties

        public string Name { get; }
        public object? Value { get; }
        public DbType Type { get; }

        #endregion

        #region Public Constructors

        public Parameter(string name, object? value, DbType type = DbType.String) {
            Name = Guard.Against.NullOrWhiteSpace(name, nameof(name));
            Value = value;
            Type = type;
        }

        #endregion

        #region Public Override Methods

        public override string ToString()
            => $"[{Type}] {Name} => {Value ?? "NULL"}";

        #endregion
    }
}