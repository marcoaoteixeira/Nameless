using System.Data;

namespace Nameless.Data {
    public static class DbConnectionExtension {
        #region Public Static Methods

        public static void EnsureOpen(this IDbConnection self) {
            if (self.State == ConnectionState.Closed) {
                self.Open();
            }
        }

        #endregion
    }
}
