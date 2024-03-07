namespace Nameless.Caching.Redis {
    /// <summary>
    /// This class was proposed to be an root point for this assembly.
    /// 
    /// *** DO NOT IMPLEMENT ANYTHING HERE ***
    /// 
    /// But, it's allowed to use it as a repository for all constants or
    /// default values that you'll use throughout this project.
    /// </summary>
    public static class Root {
        #region Public Static Inner Classes

        public static class EnvTokens {
            #region Public Static Read-Only Properties

            public const string REDIS_USER = nameof(REDIS_USER);
            public const string REDIS_PASS = nameof(REDIS_PASS);

            public const string REDIS_CERT_PASS = nameof(REDIS_CERT_PASS);

            #endregion
        }

        #endregion
    }
}
