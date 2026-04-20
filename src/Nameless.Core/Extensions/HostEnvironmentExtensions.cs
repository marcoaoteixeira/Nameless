#pragma warning disable CA1822

using Microsoft.Extensions.Hosting;

namespace Nameless;

/// <summary>
///     <see cref="IHostEnvironment" /> extension methods.
/// </summary>
public static class HostEnvironmentExtensions {
    /// <summary>
    ///     Developer machine identifier.
    /// </summary>
    public static readonly string DeveloperMachine = nameof(DeveloperMachine);

    /// <param name="self">
    /// The current instance that implements <see cref="IHostEnvironment" />.
    /// </param>
    extension(IHostEnvironment self) {
        /// <summary>
        ///     Checks if the current host environment name is <see cref="DeveloperMachine" />.
        /// </summary>
        /// <returns><see langword="true"/> if the environment name is <see cref="DeveloperMachine" />; otherwise <see langword="false"/>.</returns>
        public bool IsDeveloperMachine => self.IsEnvironment(DeveloperMachine);

        /// <summary>
        ///     Whether it is running inside a container.
        /// </summary>
        public bool IsRunningOnContainer => Environment.GetEnvironmentVariable(
            CoreConstants.EnvironmentTokens.DotnetRunningInContainer
        ).ToBoolean();

        /// <summary>
        ///     Retrieves an environment variable by its name.
        /// </summary>
        /// <param name="name">
        ///     The name.
        /// </param>
        /// <returns>
        ///     The environment variable, if exists;
        ///     otherwise <see langword="null"/>.
        /// </returns>
        public string? GetEnvironmentVariable(string name) {
            return Environment.GetEnvironmentVariable(name);
        }

        /// <summary>
        ///     Sets an environment variable.
        /// </summary>
        /// <param name="name">
        ///     The name.
        /// </param>
        /// <param name="value">
        ///     The value.
        /// </param>
        public void SetEnvironmentVariable(string name, string? value) {
            Environment.SetEnvironmentVariable(name, value);
        }
    }
}