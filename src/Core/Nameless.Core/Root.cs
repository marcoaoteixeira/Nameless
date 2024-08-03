using System.Text;

namespace Nameless {
    /// <summary>
    /// The only purpose of this class is to be an "entrypoint" for this
    /// assembly.
    /// 
    /// *** DO NOT IMPLEMENT IMPORTANT THINGS HERE ***
    /// 
    /// But, it's OK to use it as a repository for all constants or default
    /// values that you'll use throughout this assembly or shared assemblies.
    /// </summary>
    public static class Root {
        #region Public Static Inner Classes

        public static class Defaults {
            #region Public Static Read-Only Properties

            /// <summary>
            /// Gets the default encoding (UTF-8 without BOM)
            /// </summary>
            public static Encoding Encoding { get; } = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
            public static string[] OptionsSettingsTails { get; } = ["Options", "Settings"];

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

        public static class Milliseconds {
            #region Public Constants

            /// <summary>
            /// Quarter of a second expressed in milliseconds (250 ms).
            /// </summary>
            public const int QUARTER_SECOND = 250;
            /// <summary>
            /// Half of a second expressed in milliseconds (500 ms).
            /// </summary>
            public const int HALF_SECOND = 500;
            /// <summary>
            /// One second expressed in milliseconds (1000 ms).
            /// </summary>
            public const int SECOND = 1000;
            /// <summary>
            /// Quarter of a minute expressed in milliseconds (15000 ms).
            /// </summary>
            public const int QUARTER_MINUTE = 15 * SECOND;
            /// <summary>
            /// Half of a minute expressed in milliseconds (30000 ms).
            /// </summary>
            public const int HALF_MINUTE = 30 * SECOND;
            /// <summary>
            /// One minute expressed in milliseconds (60000 ms).
            /// </summary>
            public const int MINUTE = 60 * SECOND;
            /// <summary>
            /// Quarter of a hour expressed in milliseconds (900000 ms).
            /// </summary>
            public const int QUARTER_HOUR = 15 * MINUTE;
            /// <summary>
            /// Half of a hour expressed in milliseconds (1800000 ms).
            /// </summary>
            public const int HALF_HOUR = 30 * MINUTE;
            /// <summary>
            /// A full hour expressed in milliseconds (3600000 ms).
            /// </summary>
            public const int HOUR = 60 * MINUTE;
            /// <summary>
            /// Quarter of a day expressed in milliseconds (21600000 ms).
            /// </summary>
            public const int QUARTER_DAY = 6 * HOUR;
            /// <summary>
            /// Half of a day expressed in milliseconds (43200000 ms).
            /// </summary>
            public const int HALF_DAY = 12 * HOUR;
            /// <summary>
            /// A full day expressed in milliseconds (86400000 ms).
            /// </summary>
            public const int DAY = 24 * HOUR;

            #endregion
        }

        #endregion
    }
}
