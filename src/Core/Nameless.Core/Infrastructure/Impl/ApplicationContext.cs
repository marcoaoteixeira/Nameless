using Microsoft.Extensions.Hosting;

namespace Nameless.Infrastructure.Impl {
    public sealed class ApplicationContext : IApplicationContext {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="ApplicationContext"/>
        /// </summary>
        /// <param name="hostEnvironment">The host environment.</param>
        /// <param name="useAppDataSpecialFolder">
        /// If <c>true</c>, <see cref="ApplicationDataFolderPath"/> will point to:
        /// <list type="bullet">
        ///     <item>
        ///         <term>Windows</term>
        ///         <description>C:\Users\CURRENT_USER\AppData\Local\APPLICATION_NAME</description>
        ///     </item>
        ///     <item>
        ///         <term>Linux</term>
        ///         <description>/CURRENT_USER/.local/share/APPLICATION_NAME</description>
        ///     </item>
        /// </list>
        /// Otherwise, will point to <see cref="BasePath"/> + "App_Data"
        /// </param>
        /// <param name="appVersion">The application version.</param>
        public ApplicationContext(IHostEnvironment hostEnvironment, bool useAppDataSpecialFolder = true, Version? appVersion = null) {
            Guard.Against.Null(hostEnvironment, nameof(hostEnvironment));

            EnvironmentName = hostEnvironment.EnvironmentName;
            ApplicationName = hostEnvironment.ApplicationName;

            ApplicationDataFolderPath = GetApplicationDataFolder(ApplicationName, BasePath, useAppDataSpecialFolder);

            var version = appVersion ?? new Version(major: 0, minor: 0, build: 0);
            SemVer = $"v{version.Major}.{version.Minor}.{version.Build}";
        }

        #endregion

        #region Private Static Methods

        private static string GetApplicationDataFolder(string applicationName, string basePath, bool useAppDataSpecialFolder) {
            string result;

            if (useAppDataSpecialFolder) {
                var specialFolder = Environment.GetFolderPath(
                    folder: Environment.SpecialFolder.LocalApplicationData,
                    option: Environment.SpecialFolderOption.Create
                );
                
                result = Path.Combine(specialFolder, applicationName);
            } else {
                result = Path.Combine(basePath, "App_Data");
            }

            // Ensure directory exists
            Directory.CreateDirectory(result);

            return result;
        }

        #endregion

        #region IApplicationContext Members

        /// <summary>
        /// Gets the environment name. (e.g. Development, Production etc)
        /// See <see cref="IHostEnvironment.EnvironmentName"/>
        /// </summary>
        public string EnvironmentName { get; }

        /// <summary>
        /// Gets the application name.
        /// See <see cref="IHostEnvironment.ApplicationName"/>.
        /// </summary>
        public string ApplicationName { get; }

        /// <summary>
        /// Gets the base path of the application. This will always be
        /// the current location of the application assembly.
        /// </summary>
        public string BasePath => AppDomain.CurrentDomain.BaseDirectory;

        /// <summary>
        /// Gets the application data folder.
        /// </summary>
        public string ApplicationDataFolderPath { get; }

        /// <summary>
        /// Gets the semantic version of the application.
        /// </summary>
        public string SemVer { get; }

        #endregion
    }
}
