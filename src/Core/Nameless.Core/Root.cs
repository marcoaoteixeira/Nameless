using System.Text;

namespace Nameless {
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

        public static class Defaults {
            #region Public Static Read-Only Properties

            /// <summary>
            /// Gets the default encoding (UTF-8 without BOM)
            /// </summary>
            public static Encoding Encoding { get; } = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
            public const string APP_DATA_FOLDER_NAME = "App_Data";

            #endregion
        }

        public static class EnvTokens {
            #region Public Constants

            public const string DOTNET_RUNNING_IN_CONTAINER = nameof(DOTNET_RUNNING_IN_CONTAINER);

            #endregion
        }

        public static class Separators {
            #region Public Constants

            public const char SPACE = ' ';
            public const char DASH = '-';
            public const char COMMA = ',';
            public const char SEMICOLON = ';';
            public const char DOT = '.';
            public const char UNDERSCORE = '_';

            #endregion
        }

        #endregion
    }
}
