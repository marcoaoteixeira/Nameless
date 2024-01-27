using Microsoft.Extensions.Hosting;

namespace Nameless.Infrastructure.Impl {
    public sealed class ApplicationContext : IApplicationContext {
        #region Private Static Read-Only Fields

        private static readonly Version BaseVersion = new(major: 0, minor: 0, build: 0);

        #endregion

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="ApplicationContext"/>
        /// </summary>
        /// <param name="hostEnvironment">The host environment.</param>
        public ApplicationContext(IHostEnvironment hostEnvironment)
            : this(hostEnvironment, useAppDataSpecialFolder: false, appVersion: BaseVersion) { }

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
        public ApplicationContext(IHostEnvironment hostEnvironment, bool useAppDataSpecialFolder)
            : this(hostEnvironment, useAppDataSpecialFolder, appVersion: BaseVersion) { }

        /// <summary>
        /// Initializes a new instance of <see cref="ApplicationContext"/>
        /// </summary>
        /// <param name="hostEnvironment">The host environment.</param>
        /// <param name="appVersion">The application version.</param>
        public ApplicationContext(IHostEnvironment hostEnvironment, Version appVersion)
            : this(hostEnvironment, useAppDataSpecialFolder: false, appVersion) { }

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
        public ApplicationContext(IHostEnvironment hostEnvironment, bool useAppDataSpecialFolder, Version appVersion) {
            Guard.Against.Null(hostEnvironment, nameof(hostEnvironment));

            EnvironmentName = hostEnvironment.EnvironmentName;
            ApplicationName = hostEnvironment.ApplicationName;
            BasePath = GetType().Assembly.GetDirectoryPath();
            ApplicationDataFolderPath = GetApplicationDataFolder(ApplicationName, BasePath, useAppDataSpecialFolder);
            SemVer = $"v{appVersion.Major}.{appVersion.Minor}.{appVersion.Build}";
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

                var normalizedApplicationName = applicationName
                    .RemoveDiacritics()
                    .Replace(Root.Separators.SPACE, Root.Separators.UNDERSCORE)
                    .Replace(Root.Separators.DOT, Root.Separators.UNDERSCORE);

                result = Path.Combine(specialFolder, normalizedApplicationName);
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
        public string BasePath { get; }

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
