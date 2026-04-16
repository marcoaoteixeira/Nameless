using Nameless.Attributes;

namespace Nameless.WPF.GitHub;

/// <summary>
///     Defines the GitHub options for the application.
/// </summary>
[ConfigurationSectionName("GitHub")]
public class GitHubOptions {
    /// <summary>
    ///     Gets or sets the API URL.
    /// </summary>
    /// <remarks>
    ///     Default is <c>https://api.github.com</c>.
    /// </remarks>
    public string ApiBaseUrl { get; set; } = "https://api.github.com";

    /// <summary>
    ///     Gets or sets the API version.
    ///     Default value is <c>2022-11-28</c>.
    /// </summary>
    public string ApiVersion { get; set; } = "2022-11-28";

    /// <summary>
    ///     The application GitHub owner.
    /// </summary>
    public string Owner { get; set; } = string.Empty;

    /// <summary>
    ///     The application GitHub repository name.
    /// </summary>
    public string Repository { get; set; } = string.Empty;
}
