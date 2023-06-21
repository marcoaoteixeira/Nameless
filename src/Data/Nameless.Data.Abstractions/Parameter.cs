using System.Data;

namespace Nameless.Data {

    public record Parameter(string Name, object? Value, DbType Type = DbType.String) {
        #region Public Override Methods

        public override string ToString() {
            return $"[{Type}] {Name} => {Value}";
        }

        #endregion
    }
}