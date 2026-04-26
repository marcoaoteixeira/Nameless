using Nameless.Attributes;

namespace Nameless.Auth.OAuth;

/// <summary>
///     Represents the OAuth configuration options.
/// </summary>
[ConfigurationSectionName("OAuth")]
public record OAuthOptions {
    /// <summary>
    ///     Gets the authority URL.
    /// </summary>
    public string AuthorityUrl { get; init; } = string.Empty;

    /// <summary>
    ///     Gets a dictionary of values to use in the request header.
    /// </summary>
    public Dictionary<string, string?> Header { get; init; } = [];
}
