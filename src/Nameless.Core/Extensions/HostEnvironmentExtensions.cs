using Microsoft.Extensions.Hosting;

namespace Nameless;

/// <summary>
///     <see cref="IHostEnvironment" /> extension methods.
/// </summary>
public static class HostEnvironmentExtensions {
    public static readonly string DeveloperMachine = nameof(DeveloperMachine);

    /// <param name="self">The current instance that implements <see cref="IHostEnvironment" />.</param>
    extension(IHostEnvironment self) {
        /// <summary>
        ///     Checks if the current host environment name is <see cref="DeveloperMachine" />.
        /// </summary>
        /// <returns><see langword="true"/> if the environment name is <see cref="DeveloperMachine" />; otherwise <see langword="false"/>.</returns>
        public bool IsDeveloperMachine() {
            return self.IsEnvironment(DeveloperMachine);
        }

        public bool IsRunningOnContainer() {
            return Environment.GetEnvironmentVariable(EnvironmentTokens.DOTNET_RUNNING_IN_CONTAINER)
                              .ToBoolean();
        }

        public string? GetEnvironmentVariable(string name) {
            return Environment.GetEnvironmentVariable(name);
        }

        public void SetEnvironmentVariable(string name, string? value) {
            Environment.SetEnvironmentVariable(name, value);
        }
    }
}