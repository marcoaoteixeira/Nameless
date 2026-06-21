using Microsoft.Extensions.Configuration;

namespace Nameless.Testing.Tools.Helpers;

/// <summary>
///     Configuration helper.
/// </summary>
public static class ConfigurationHelper {
    /// <summary>
    ///     Creates a <see cref="IConfiguration"/> object with data
    ///     from <paramref name="inMemory"/>, environment and <c>AppSettings.json</c> file.
    /// </summary>
    /// <param name="inMemory">
    ///     The in-memory data.
    /// </param>
    /// <returns>
    ///     A new instance of <see cref="IConfiguration"/>.
    /// </returns>
    public static IConfiguration CreateConfiguration(Dictionary<string, string?>? inMemory = null) {
        return new ConfigurationBuilder()
               .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
               .AddEnvironmentVariables()
               .AddJsonFile("AppSettings.json", optional: true)
               .AddInMemoryCollection(inMemory)
               .Build();
    }
}