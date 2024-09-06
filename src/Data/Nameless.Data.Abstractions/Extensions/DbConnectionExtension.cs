using System.Data;

namespace Nameless.Data;

public static class DbConnectionExtension {
    public static void EnsureOpen(this IDbConnection self) {
        Prevent.Argument.Null(self, nameof(self));

        if (self.State == ConnectionState.Closed) {
            self.Open();
        }
    }
}