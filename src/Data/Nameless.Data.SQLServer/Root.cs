namespace Nameless.Data.SQLServer {
    /// <summary>
    /// This class was defined to be an entrypoint for this project assembly.
    /// 
    /// *** DO NOT IMPLEMENT ANYTHING HERE ***
    /// 
    /// But, it's allow to use this class as a repository for all constants or
    /// default values that we'll use throughout this project.
    /// </summary>
    public static class Root {
        #region Public Static Inner Classes

        public static class EnvTokens {
            #region Public Static Read-Only Properties

            public const string SQLSERVER_USER = nameof(SQLSERVER_USER);
            public const string SQLSERVER_PASS = nameof(SQLSERVER_PASS);

            #endregion
        }

        #endregion

        #region Internal Static Inner Classes

        internal static class Defaults {
            #region Internal Static Read-Only Properties

            internal const string SQLSERVER_USER = "sa";
            internal const string SQLSERVER_PASS = "123456@AbC";

            #endregion
        }

        #endregion
    }
}
