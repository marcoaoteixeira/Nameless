namespace Nameless;

public static class Constants {
    public static class EnvTokens {
        public const string DOTNET_RUNNING_IN_CONTAINER = nameof(DOTNET_RUNNING_IN_CONTAINER);
    }

    public static class Separators {
        public const string SPACE = " ";
        public const string DASH = "-";
        public const string COMMA = ",";
        public const string SEMICOLON = ";";
        public const string DOT = ".";
        public const string UNDERSCORE = "_";
        public const string SHARP = "#";
    }

    public static class Milliseconds {
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
        /// Quarter of an hour expressed in milliseconds (900000 ms).
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
    }
}
