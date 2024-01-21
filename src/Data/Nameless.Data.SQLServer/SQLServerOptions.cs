#if NET6_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif

namespace Nameless.Data.SQLServer {
    public sealed class SQLServerOptions {
        #region Public Static Read-Only Properties

        public static SQLServerOptions Default => new();

        #endregion

        #region Public Constructors

        public SQLServerOptions() {
            UserName = Environment.GetEnvironmentVariable(Root.EnvTokens.SQLSERVER_USER)
                ?? string.Empty;
            Password = Environment.GetEnvironmentVariable(Root.EnvTokens.SQLSERVER_PASS)
                ?? string.Empty;
        }

        #endregion

        #region Public Properties

        public string Server { get; set; } = "(localdb)\\MSSQLLocalDB";
        public string Database { get; set; } = "master";
        public string UserName { get; }
        public string Password { get; }
#if NET6_0_OR_GREATER
        [MemberNotNullWhen(true, nameof(UserName), nameof(Password))]
#endif
        public bool UseCredentials
            => !string.IsNullOrWhiteSpace(UserName) &&
               !string.IsNullOrWhiteSpace(Password);
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

            connStr += UseCredentials
                ? $"User Id={UserName};Password={Password};"
                : string.Empty;

            connStr += UseIntegratedSecurity
                ? "Integrated Security=true;"
                : string.Empty;

            return connStr;
        }

        #endregion

        #region Public Override Methods

        public override string ToString()
            => GetConnectionString();

        #endregion
    }
}
