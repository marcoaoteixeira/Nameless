using System.Data;

namespace Nameless.Data {

    public record Parameter(string Name, object? Value, DbType Type = DbType.String, ParameterDirection Direction = ParameterDirection.Input) {
        #region Public Override Methods

        public override string ToString() {
            return $"[{Direction}] ({Type}) {Name} => {Value}";
        }

        #endregion
    }
}