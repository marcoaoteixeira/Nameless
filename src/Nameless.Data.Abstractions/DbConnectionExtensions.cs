using System.Data;

namespace Nameless.Data;

/// <summary>
///     Extensions for <see cref="IDbConnection"/>.
/// </summary>
public static class DbConnectionExtensions {
    /// <summary>
    ///     Ensures that the connection is open. If it is closed, it will open the connection.
    /// </summary>
    /// <param name="self">
    ///     The current <see cref="IDbConnection"/> instance.
    /// </param>
    public static void EnsureOpen(this IDbConnection self) {
        if (self.State == ConnectionState.Closed) {
            self.Open();
        }
    }
}