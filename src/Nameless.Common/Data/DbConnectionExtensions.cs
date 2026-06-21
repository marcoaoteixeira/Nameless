using System.Data;

namespace Nameless.Data;

/// <summary>
///     Extensions for <see cref="IDbConnection"/>.
/// </summary>
public static class DbConnectionExtensions {
    /// <param name="self">
    ///     The current <see cref="IDbConnection"/> instance.
    /// </param>
    extension(IDbConnection self) {
        /// <summary>
        ///     Ensures that the connection is open. If it is closed, it will open the connection.
        /// </summary>
        public void EnsureOpen() {
            if (self.State == ConnectionState.Closed) {
                self.Open();
            }
        }
    }
}