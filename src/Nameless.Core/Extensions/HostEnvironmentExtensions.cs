using Microsoft.Extensions.Hosting;

namespace Nameless;

/// <summary>
///     <see cref="IHostEnvironment" /> extension methods.
/// </summary>
public static class HostEnvironmentExtensions {
    public static readonly string DeveloperMachine = nameof(DeveloperMachine);

    /// <summary>
    ///     Checks if the current host environment name is <see cref="DeveloperMachine" />.
    /// </summary>
    /// <param name="self">The current instance that implements <see cref="IHostEnvironment" />.</param>
    /// <returns><see langword="true"/> if the environment name is <see cref="DeveloperMachine" />; otherwise <see langword="false"/>.</returns>
    public static bool IsDeveloperMachine(this IHostEnvironment self) {
        return self.IsEnvironment(DeveloperMachine);
    }

    public static bool IsRunningOnContainer(this IHostEnvironment _) {
        return Environment.GetEnvironmentVariable(EnvironmentTokens.DOTNET_RUNNING_IN_CONTAINER)
                          .ToBoolean();
    }

    public static string? GetEnvironmentVariable(this IHostEnvironment _, string name) {
        return Environment.GetEnvironmentVariable(name);
    }

    public static void SetEnvironmentVariable(this IHostEnvironment _, string name, string? value) {
        Environment.SetEnvironmentVariable(name, value);
    }
}