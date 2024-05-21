#if NET6_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif

namespace Nameless.Data.SQLServer.Options {
    public sealed class SQLServerOptions {
        #region Public Static Read-Only Properties

        public static SQLServerOptions Default => new();

        #endregion

        #region Public Properties

        public string Server { get; set; } = "(localdb)\\MSSQLLocalDB";
        public string Database { get; set; } = "master";
        public string Username { get; } = Environment.GetEnvironmentVariable(Root.EnvTokens.SQLSERVER_USER) ?? string.Empty;
        public string Password { get; } = Environment.GetEnvironmentVariable(Root.EnvTokens.SQLSERVER_PASS) ?? string.Empty;
        public bool UseAttachedDb { get; set; }
        public bool UseIntegratedSecurity { get; set; }

        #endregion

        #region Public Methods

        public string GetConnectionString() {
            var connStr = string.Empty;

            connStr += $"Server={Server};";

            connStr += UseAttachedDb
                ? $"AttachDbFilename={Database};"
                : $"Database={Database};";

            connStr += UseCredentials()
                ? $"User Id={Username};Password={Password};"
                : string.Empty;

            connStr += UseIntegratedSecurity
                ? "Integrated Security=true;"
                : string.Empty;

            return connStr;
        }

#if NET6_0_OR_GREATER
        [MemberNotNullWhen(true, nameof(Username), nameof(Password))]
#endif
        public bool UseCredentials()
            => !string.IsNullOrWhiteSpace(Username) &&
               !string.IsNullOrWhiteSpace(Password) &&
               !UseIntegratedSecurity;

        #endregion
    }
}
