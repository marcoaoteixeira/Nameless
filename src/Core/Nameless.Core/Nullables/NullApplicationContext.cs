using Nameless.Infrastructure;

namespace Nameless {
    [Singleton]
    public sealed class NullApplicationContext : IApplicationContext {
        #region Public Static Properties

        /// <summary>
        /// Gets the unique instance of <see cref="NullDisposable" />.
        /// </summary>
        public static IApplicationContext Instance { get; } = new NullApplicationContext();

        #endregion

        #region Static Constructors

        // Explicit static constructor to tell the C# compiler
        // not to mark type as beforefieldinit
        static NullApplicationContext() { }

        #endregion

        #region Private Constructors

        private NullApplicationContext() {
            var assembly = typeof(NullApplicationContext).Assembly;

            BasePath = assembly.GetDirectoryPath();

            ApplicationDataFolderPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData, Environment.SpecialFolderOption.Create),
                ApplicationName
            );

            var version = assembly
                .GetName()
                .Version ?? new(major: 1, minor: 0, build: 0);

            SemVer = $"{version.Major}.{version.Minor}.{version.Build}";
        }

        #endregion

        #region IApplicationContext Members

        public string EnvironmentName => "Development";
        public string ApplicationName => "NullApplication";
        public string BasePath { get; }
        public string ApplicationDataFolderPath { get; }
        public string SemVer { get; }

        #endregion
    }
}
