﻿using System.Diagnostics.CodeAnalysis;

namespace Nameless.Data.SQLite {
    public sealed class SQLiteOptions {
        #region Public Static Read-Only Properties

        public static SQLiteOptions Default => new();

        #endregion

        #region Public Constructors

        public SQLiteOptions() {
            Password = Environment.GetEnvironmentVariable(Root.EnvTokens.SQLITE_PASS)
                ?? Root.Defaults.SQLITE_PASS;
        }

        #endregion

        #region Public Properties

        public bool UseInMemory { get; set; }
        public string DatabaseName { get; set; } = "database";
        public string Password { get; }
        [MemberNotNullWhen(true, nameof(Password))]
        public bool UseCredentials
            => !string.IsNullOrWhiteSpace(Password);

        #endregion

        #region Public Methods

        public string GetConnectionString() {
            var connStr = string.Empty;

            connStr += $"Data Source={(UseInMemory ? DatabaseName : ":memory:")};";

            connStr += UseCredentials
                ? $"Password={Password};"
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