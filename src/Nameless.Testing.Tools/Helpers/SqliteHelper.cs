using System.Data.Common;
using Microsoft.Data.Sqlite;

namespace Nameless.Testing.Tools.Helpers;

public static class SqliteHelper {
    public static string RootPath { get; } = Path.Combine(typeof(SqliteHelper).Assembly.GetDirectoryPath(), "__sqlite__");

    static SqliteHelper() {
        // Ensure the root path exists
        Directory.CreateDirectory(RootPath);
    }

    /// <summary>
    ///     Creates and returns a new database connection, optionally targeting
    ///     the specified database file.
    /// </summary>
    /// <param name="fileName">
    ///     The path to the database file to connect.
    ///     If <see langword="null"/>, a random name will be generated.
    /// </param>
    /// <returns>
    ///     A new instance of a <see cref="DbConnection"/> configured to connect
    ///     to the specified SQLite database file.
    /// </returns>
    public static DbConnection CreateDbConnection(string? fileName = null) {
        fileName = string.IsNullOrWhiteSpace(fileName)
            ? $"{Guid.CreateVersion7():N}.db"
            : fileName;

        var filePath = Path.Combine(RootPath, fileName);
        var connStr = $"Data Source={filePath}";

        return new SqliteConnection(connStr);
    }
}