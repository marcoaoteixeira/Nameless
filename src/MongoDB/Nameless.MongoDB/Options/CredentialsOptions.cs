﻿#if NET6_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif

namespace Nameless.MongoDB.Options {
    public sealed class CredentialsOptions {
        #region Public Static Read-Only Properties

        public static CredentialsOptions Default => new();

        #endregion

        #region Public Constructors

        public CredentialsOptions() {
            Database = Environment.GetEnvironmentVariable(Root.EnvTokens.MONGO_CRED_DATABASE)
                ?? Root.Defaults.MONGO_CRED_DATABASE;
            Mechanism = Environment.GetEnvironmentVariable(Root.EnvTokens.MONGO_CRED_MECHANISM)
                ?? Root.Defaults.MONGO_CRED_MECHANISM;
            Username = Environment.GetEnvironmentVariable(Root.EnvTokens.MONGO_CRED_USER)
                ?? Root.Defaults.MONGO_CRED_USER;
            Password = Environment.GetEnvironmentVariable(Root.EnvTokens.MONGO_CRED_PASS)
                ?? Root.Defaults.MONGO_CRED_PASS;
        }

        #endregion

        #region Public Properties

        public string Database { get; }

        public string Mechanism { get; }

        public string Username { get; }

        public string Password { get; }

        #endregion

        #region Public Methods

#if NET6_0_OR_GREATER
        [MemberNotNullWhen(true, nameof(Database), nameof(Mechanism), nameof(Username), nameof(Password))]
#endif
        public bool UseCredentials()
                    => !string.IsNullOrWhiteSpace(Database) &&
                       !string.IsNullOrWhiteSpace(Mechanism) &&
                       !string.IsNullOrWhiteSpace(Username) &&
                       !string.IsNullOrWhiteSpace(Password);

        #endregion
    }
}