using System.Diagnostics.CodeAnalysis;

namespace Nameless.Data.SQLServer {
    public sealed class SQLServerOptions {
        #region Public Static Read-Only Properties

        public static SQLServerOptions Default => new();

        #endregion

        #region Public Constructors

        public SQLServerOptions() {
            UserName = Environment.GetEnvironmentVariable(Root.EnvTokens.SQLSERVER_USER)
                ?? Root.Defaults.SQLSERVER_USER;
            Password = Environment.GetEnvironmentVariable(Root.EnvTokens.SQLSERVER_PASS)
                ?? Root.Defaults.SQLSERVER_PASS;
        }

        #endregion

        #region Public Properties

        public string Server { get; set; } = "(localdb)\\MSSQLLocalDB";
        public string Database { get; set; } = "master";
        public string UserName { get; }
        public string Password { get; }
        [MemberNotNullWhen(true, nameof(UserName), nameof(Password))]
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
