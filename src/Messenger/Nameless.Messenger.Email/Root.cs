namespace Nameless.Messenger.Email {
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
            #region Public Constants

            public const string MESSENGER_USER = nameof(MESSENGER_USER);
            public const string MESSENGER_PASS = nameof(MESSENGER_PASS);

            #endregion
        }

        public static class Defaults {
            #region Public Static Methods

            public static string FileNameGenerator()
                => $"{DateTime.Now:yyyyMMddHHmmss}_{Guid.NewGuid():N}.eml";

            #endregion
        }

        #endregion
    }
}
