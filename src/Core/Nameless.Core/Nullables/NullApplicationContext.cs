using Nameless.Infrastructure;

namespace Nameless {
    [Singleton]
    public sealed class NullApplicationContext : IApplicationContext {
        #region Private Static Read-Only Fields

        private static readonly NullApplicationContext _instance = new();

        #endregion

        #region Public Static Properties

        /// <summary>
        /// Gets the unique instance of <see cref="NullDisposable" />.
        /// </summary>
        public static IApplicationContext Instance => _instance;

        #endregion

        #region Static Constructors

        // Explicit static constructor to tell the C# compiler
        // not to mark type as beforefieldinit
        static NullApplicationContext() { }

        #endregion

        #region Private Constructors

        private NullApplicationContext() { }

        #endregion

        #region IApplicationContext Members

        public string EnvironmentName => "Development";

        public string ApplicationName => "Application";

        public string BasePath => typeof(NullApplicationContext).Assembly.GetDirectoryPath();

        public string DataDirectoryPath => Path.Combine(BasePath, "App_Data");

        #endregion
    }
}
