using Microsoft.Extensions.Hosting;

namespace Nameless.AspNetCore {

    /// <summary>
    /// <see cref="IHostEnvironment"/> extension methods.
    /// </summary>
    public static class HostEnvironmentExtension {

        #region Public Static Read-Only Fields

        public static readonly string DeveloperMachine = nameof(DeveloperMachine);

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Checks if the current host environment name is <see cref="DeveloperMachine"/>.
        /// </summary>
        /// <param name="self">The current instance that implements <see cref="IHostEnvironment"/>.</param>
        /// <returns><c>true</c> if the environment name is <see cref="DeveloperMachine"/>; otherwise <c>false</c>.</returns>
        public static bool IsDeveloperMachine(this IHostEnvironment self)
            => self != default && self.IsEnvironment(DeveloperMachine);

        #endregion
    }
}
