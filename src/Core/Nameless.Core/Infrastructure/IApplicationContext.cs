namespace Nameless.Infrastructure {
    public interface IApplicationContext {
        #region Properties

        /// <summary>
        /// Gets the current environment name.
        /// </summary>
        string EnvironmentName { get; }
        /// <summary>
        /// Gets the application name.
        /// </summary>
        string ApplicationName { get; }
        /// <summary>
        /// Gets the (full) path to the executing application.
        /// </summary>
        string BasePath { get; }

        #endregion
    }
}
