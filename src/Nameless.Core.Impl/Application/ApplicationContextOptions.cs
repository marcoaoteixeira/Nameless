using System.Diagnostics.CodeAnalysis;
using Nameless.Attributes;

namespace Nameless.Application;

/// <summary>
///     Application context options.
/// </summary>
[ExcludeFromCodeCoverage(Justification = CodeCoverage.Justifications.Poco)]
[ConfigurationSectionName("ApplicationContext")]
public record ApplicationContextOptions {
    /// <summary>
    ///     Gets or sets the name of the current environment.
    /// </summary>
    public string EnvironmentName { get; init; } = string.Empty;

    /// <summary>
    ///     Gets or sets the name of the application.
    /// </summary>
    public string ApplicationName { get; init; } = AppDomain.CurrentDomain.FriendlyName;

    /// <summary>
    ///     Gets or sets where the application should store its data.
    /// </summary>
    public ApplicationDataLocation ApplicationDataLocation { get; init; }

    /// <summary>
    ///     Gets or sets the version of the application.
    /// </summary>
    public SemanticVersion Version { get; init; } = new(major: 1, minor: 0, patch: 0);
}