#if NET6_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif

namespace Nameless.Data.SQLite.Options {
    public sealed class SQLiteOptions {
        #region Public Static Read-Only Properties

        public static SQLiteOptions Default => new();

        #endregion

        #region Public Properties

        public bool UseInMemory { get; set; }
        
        public string DatabasePath { get; set; } = $".{Path.DirectorySeparatorChar}database.db";
        
        public string Password { get; }

        #endregion

        #region Public Constructors

        public SQLiteOptions() {
            Password = Environment.GetEnvironmentVariable(Root.EnvTokens.SQLITE_PASS)
                ?? string.Empty;
        }

        #endregion

        #region Public Methods

        public string GetConnectionString() {
            var connStr = string.Empty;

            connStr += $"Data Source={(UseInMemory ? ":memory:" : DatabasePath)};";
            connStr += UseCredentials()
                ? $"Password={Password};"
                : string.Empty;

            return connStr;
        }

#if NET6_0_OR_GREATER
        [MemberNotNullWhen(true, nameof(Password))]
#endif
        public bool UseCredentials()
            => !string.IsNullOrWhiteSpace(Password);

        #endregion

        #region Public Override Methods

        public override string ToString()
            => GetConnectionString();

        #endregion
    }
}
