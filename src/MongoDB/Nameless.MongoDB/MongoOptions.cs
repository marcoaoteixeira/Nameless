#if NET6_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif

namespace Nameless.MongoDB {
    public sealed class MongoOptions {
        #region Public Static Read-Only Properties

        public static MongoOptions Default => new();

        #endregion

        #region Public Constructors

        public MongoOptions() {
            UserName = Environment.GetEnvironmentVariable(Root.EnvTokens.MONGO_USER)
                ?? Root.Defaults.MONGO_USER;
            Password = Environment.GetEnvironmentVariable(Root.EnvTokens.MONGO_PASS)
                ?? Root.Defaults.MONGO_PASS;
        }

        #endregion

        #region Public Properties

        public string Host { get; set; } = "mongodb://mongodb0.example.com";
        public int Port { get; set; } = 27017;
        public string Database { get; set; } = "Default";
        public string UserName { get; }
        public string Password { get; }

#if NET6_0_OR_GREATER
        [MemberNotNullWhen(true, nameof(UserName), nameof(Password))]
#endif
        public bool UseCredentials
                    => !string.IsNullOrWhiteSpace(UserName) &&
                       !string.IsNullOrWhiteSpace(Password);

        #endregion
    }
}
