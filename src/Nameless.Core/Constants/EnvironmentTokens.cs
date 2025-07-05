namespace Nameless;

/// <summary>
///     Static class that holds parameters' names for environment variables.
/// </summary>
public static class EnvironmentTokens {
    /// <summary>
    ///     EnvironmentName parameter: DOTNET_RUNNING_IN_CONTAINER
    /// </summary>
    /// <remarks>
    ///     Commonly used in web applications that runs in containers.
    /// </remarks>
    public const string DOTNET_RUNNING_IN_CONTAINER = nameof(DOTNET_RUNNING_IN_CONTAINER);
}